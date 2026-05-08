using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

// ILoggerService arayüzünün amacı, loglama işlemleri için servis sözleşmesi tanımlamaktır.
// Info, warning, error log yazma ve logları listeleme metotlarını belirler.
// SOLID: ISP kullanır; sadece loglama servis ihtiyaçlarını içerir.
// SOLID: DIP sağlar; API katmanı somut logger yerine bu arayüze bağlı kalır.

public interface ILoggerService
{
    Task LogInfoAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null);

    Task LogWarningAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null);

    Task LogErrorAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null, string? exceptionDetail = null);

    // Adminin verdigi filtreyle log listesini sayfali sekilde getirir.
    Task<IEnumerable<LogDto>> GetLogsAsync(LogFilterDto filter);

    // Toplam kayit sayisi (filtre uygulanmis halde) — sayfalama icin gerekli.
    Task<int> GetLogCountAsync(LogFilterDto filter);

    // Tek log kaydini id ile getirir; bulunamazsa null doner.
    Task<LogDto?> GetLogByIdAsync(int id);
}
