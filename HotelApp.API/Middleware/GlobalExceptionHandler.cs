using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Diagnostics;
using System.Security.Claims;

// GlobalExceptionHandler, yakalanmayan hatalari tek yerde toplar.
// Hata oldugunda hem log yazar hem de kullaniciya standart bir 500 cevabi doner.
namespace HotelApp.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    // Log kaydi icin logger servisi
    private readonly ILoggerService _logger;

    // Logger servisini DI ile aliyoruz
    public GlobalExceptionHandler(ILoggerService logger)
    {
        _logger = logger;
    }

    // Uygulamada yakalanmayan bir hata olursa bu metod calisir.
    // true donerse, hata burada handle edilmis sayilir.
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Hatayi veritabanina logla
        await LogExceptionAsync(httpContext, exception);

        // Kullaniciya standart hata cevabi don
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Sunucu tarafinda bir hata olustu. Lutfen daha sonra tekrar deneyin."
            // Guvenlik icin hata detayini disariya donmuyoruz.
        }, cancellationToken);

        return true;
    }

    // Log icin gerekli ek bilgileri topla ve logger'a gonder
    private async Task LogExceptionAsync(HttpContext context, Exception ex)
    {
        // Hatanin hangi controller ve action'da oldugunu al
        var routeValues = context.Request.RouteValues;
        var controller = routeValues["controller"]?.ToString() ?? "UnknownController";
        var action = routeValues["action"]?.ToString() ?? $"{context.Request.Method} {context.Request.Path}";

        // Istek yapan kullanicinin IP bilgisini al
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        // Giris yapan kullanici varsa user bilgisini al
        var userIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? userId = int.TryParse(userIdStr, out var id) ? id : null;
        var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;

        await _logger.LogErrorAsync(
            message: ex.Message,
            userId: userId,
            userEmail: userEmail,
            controller: controller,
            action: action,
            ipAddress: ipAddress,
            exceptionDetail: ex.ToString()  // Hatanin detayli metni
        );
    }
}
