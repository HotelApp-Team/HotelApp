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

        var statusCode = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound, //kaynak bulunamadi hatasi
            InvalidOperationException => StatusCodes.Status409Conflict, // kural ihlali, onceden var olan kaynakla ilgili hata
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized, // yetkisiz erisim hatasi
            ArgumentException => StatusCodes.Status400BadRequest, // gecersiz parametre
            _ => StatusCodes.Status500InternalServerError // diger tum hatalar icin genel sunucu hatasi
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            // Sadece 500 hatalarini logluyoruz, cunku digerleri genellikle istemci hatasi ve beklenen durumlar olabilir.
            await LogExceptionAsync(httpContext, exception);
        }

        // Kullaniciya standart hata cevabi don
        httpContext.Response.StatusCode = statusCode;
        var message = statusCode == StatusCodes.Status500InternalServerError
             ? "Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyiniz."
             : exception.Message; // 500 olmayan hatalarda orijinal mesajı dönebiliriz

        await httpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = message
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
