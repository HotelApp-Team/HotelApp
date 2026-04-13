using HotelApp.Domain.Interfaces;
using HotelApp.Domain.Services;
using HotelApp.Infrastructure;
using HotelApp.Infrastructure.Context;
using HotelApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Servisleri container'a ekle.

builder.Services.AddControllers();

builder.Services.AddDbContext<HotelAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT ayarlarini oku (DI kayitlarinda ve Authentication'da kullanilacak)
var jwtSettings = builder.Configuration.GetSection("Jwt"); //appsettings.json'daki jwt bolgunu okur

// Auth katmani DI kayitlari
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); //Her HTTP istegi icin yeni instance, istek bitince yok edilir.
builder.Services.AddScoped<IAuthService, AuthService>(); //Her HTTP istegi icin yeni instance, istek bitince yok edilir.
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IJwtService>(sp =>
    new JwtService(
        secretKey: jwtSettings["SecretKey"]!,
        issuer: jwtSettings["Issuer"]!,
        audience: jwtSettings["Audience"]!,
        expirationMinutes: int.Parse(jwtSettings["ExpirationMinutes"]!)
    ));
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!); //secretkeyi byte array'e cevirir (cunku imzalama byte ister)

builder.Services.AddAuthentication(options => //varsiylan auth JWT Bearer'der
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => //Token'i nasil dogrulayacagini soyler
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, //tokenin dogru sunucudan geldigini kontrol eder
        ValidateAudience = true, //toknein dogru alici icin oldugunu kontrol eder
        ValidateLifetime = true, //token suresini kontrol eder
        ValidateIssuerSigningKey = true, //imzayi kontrol eder sahte mi degil mi anlamamizi saglar
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero //tokenin gecerlilik suresine eklenen ekstra zaman (default 5 dakika) bu ornekte sifir yaparak tokenin tam suresinde gecersiz olmasini sagliyoruz
    };
});

// CORS policy (development - genis izin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy => //bir policy olusturur.
    {
        policy.AllowAnyOrigin() //her domain porttan gelen istegi kabul eder.
              .AllowAnyMethod() //her HTTP metodunu kabul eder
              .AllowAnyHeader(); // authorization headeri gibi her turlu headeri kabul eder.

        //Onemli not: AllowAnyOrigin() ile AllowCredentials() ayni anda kullanilamaz (guvenlik gereksinimi). Cookie/credential bazli auth lazim olursa policy'yi degistiririz, ama JWT header bazli oldugu icin bu sorun olmayacak.
    });
});

// Swagger / OpenAPI yapilandirmasi hakkinda daha fazla bilgi icin: https://aka.ms/aspnetcore/swashbucklev https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo //apiye baslik verir
    {
        Title = "HotelApp API",
        Version = "v1",
        Description = "Otel Rezervasyon Web Uygulamasi API"
    });

    // JWT Bearer security tanimi
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme //Swagger'a JWT Bearer auth kullancagim der
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'i 'Bearer ' on eki OLMADAN buraya yapistir."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement // tum endpointlere auth uygula
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

// HTTP istek hattini yapilandir.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DevelopmentPolicy");

app.UseAuthentication(); //Authentication'i kullanmaya basla (JWT tokenlarini dogrulamak icin)
app.UseAuthorization();

app.MapControllers();

app.Run();
