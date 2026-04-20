using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

public interface ILoggerService
{
    Task LogInfoAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null);

    Task LogWarningAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null);

    Task LogErrorAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null, string? exceptionDetail = null);

    Task<IEnumerable<LogDto>> GetLogsAsync(int pageNumber = 1, int pageSize = 20);

    Task<int> GetLogCountAsync();
}
