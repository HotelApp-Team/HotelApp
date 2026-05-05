using HotelApp.Domain.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Diagnostics;

namespace HotelApp.API.Middleware;

// GlobalExceptionHandler sınıfının amacı, uygulamadaki yakalanmayan hataları tek yerde yönetmektir.
// Hataları uygun HTTP durum koduna çevirir, gerekli durumlarda loglar ve standart hata cevabı döner.
// SOLID: SRP kullanır; sadece merkezi hata yönetiminden sorumludur.
// SOLID: DIP kullanır; loglama için somut sınıf yerine ILoggerService arayüzüne bağlıdır.

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

        if (statusCode == StatusCodes.Status500InternalServerError
            || exception is InvalidOperationException)
        {
            // 500 hatalarini ve InvalidOperationException'i logluyoruz.
            // InvalidOperationException is kurali gibi gorunse de framework icinden de gelebilir (orn: LINQ Single bos liste).
            // Yanlis yerden gelirse fark edebilmek icin log'a dusurelim.
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
        // Exception handler middleware pipeline'i re-execute ettigi icin context.GetEndpoint() null doner.
        // Orijinal endpoint bilgisi IExceptionHandlerFeature icinde saklanir, oradan okuyoruz.
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var endpoint = exceptionFeature?.Endpoint ?? context.GetEndpoint();
        var routeValues = exceptionFeature?.RouteValues ?? context.Request.RouteValues;

        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var controller = actionDescriptor?.ControllerName
            ?? routeValues?["controller"]?.ToString()
            ?? "UnknownController";
        var action = actionDescriptor?.ActionName
            ?? routeValues?["action"]?.ToString()
            ?? $"{context.Request.Method} {context.Request.Path}";

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
