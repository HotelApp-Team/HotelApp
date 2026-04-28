using HotelApp.API.Middleware;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Domain.Services;
using HotelApp.Infrastructure;
using HotelApp.Infrastructure.Context;
using HotelApp.Infrastructure.Repositories;
using HotelApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Servisleri container'a ekle.
builder.Services.AddControllers();

// Global exception handler kaydi (.NET 8 IExceptionHandler pattern)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ProblemDetails - hata cevaplari icin standart format saglar
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<HotelAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// LoggerService Singleton baslatma ve DI kaydi
LoggerService.Initialize(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddSingleton<ILoggerService>(LoggerService.Instance);

// JWT ayarlarini oku (DI kayitlarinda ve Authentication'da kullanilacak)
var jwtSettings = builder.Configuration.GetSection("Jwt");

// Email ayarlarini oku (IEmailService kaydinda kullanilacak)
var emailSettings = builder.Configuration.GetSection("EmailSettings");

// Auth ayarlarini oku (Faz 1 / Faz 2 fallback davranisinda kullanilacak)
var authSettings = builder.Configuration.GetSection("AuthSettings");

// UnitOfWork kaydi
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repository kayitlari
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelImageRepository, HotelImageRepository>();
builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
builder.Services.AddScoped<IHotelRoomRepository, HotelRoomRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();


// Service kayitlari
builder.Services.AddScoped<IAuthService>(sp =>
    new AuthService(
        sp.GetRequiredService<IUnitOfWork>(),
        sp.GetRequiredService<IJwtService>(),
        sp.GetRequiredService<IEmailService>(),
        bool.Parse(authSettings["UseEmailVerification"] ?? "true")
    ));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IHotelImageService, HotelImageService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IHotelRoomService, HotelRoomService>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Yardimci service kayitlari
builder.Services.AddScoped<IJwtService>(sp =>
    new JwtService(
        secretKey: jwtSettings["SecretKey"]!,
        issuer: jwtSettings["Issuer"]!,
        audience: jwtSettings["Audience"]!,
        expirationMinutes: int.Parse(jwtSettings["ExpirationMinutes"]!),
        rememberMeTokenExpiryDays: int.Parse(jwtSettings["RememberMeTokenExpiryDays"]!)
    ));

builder.Services.AddScoped<IEmailService>(sp =>
    new EmailService(
        smtpServer: emailSettings["SmtpServer"]!,
        smtpPort: int.Parse(emailSettings["SmtpPort"]!),
        senderEmail: emailSettings["SenderEmail"]!,
        senderName: emailSettings["SenderName"]!,
        username: emailSettings["Username"]!,
        password: emailSettings["Password"]!,
        verifyEmailBaseUrl: emailSettings["VerifyEmailBaseUrl"]!
    ));

var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HotelApp API",
        Version = "v1",
        Description = "Otel Rezervasyon Web Uygulamasi API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'i 'Bearer ' on eki OLMADAN buraya yapistir."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Role seed: yeni bir ortamda Roles tablosu bos ise register akisi patlamasin diye
// uygulama baslarken "Admin" ve "User" rollerini garanti ediyoruz.
// Idempotent: zaten varsa dokunmaz, yoksa ekler. DB First ile uyumlu, sadece veri ekler.
// DbContext scoped oldugu icin app.Services'den direkt alamayiz, bu yuzden CreateScope() kullaniyoruz.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelAppDbContext>();

    string[] requiredRoles = { "Admin", "User" };

    foreach (var roleName in requiredRoles)
    {
        bool exists = db.Roles.Any(r => r.Name == roleName);
        if (!exists)
        {
            db.Roles.Add(new Role { Name = roleName });
        }
    }

    db.SaveChanges();
}

// Exception handler EN BASTA olmali ki sonraki tum middleware/controller hatalarini yakalasin
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
    app.UseCors("DevelopmentPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
