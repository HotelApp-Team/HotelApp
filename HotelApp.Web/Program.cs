using HotelApp.Web.Filters;
using HotelApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session (JWT token'i saklamak icin)
builder.Services.AddSession();

// Filterlar
builder.Services.AddScoped<AuthFilter>();
builder.Services.AddScoped<AdminFilter>();

builder.Services.AddHttpContextAccessor();

// ApiService (HttpClient wrapper)
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Session'i kullan

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
