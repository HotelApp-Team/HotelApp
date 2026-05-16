using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardDataAsync();

    // Admin paneli için kullanıcıları filtreli, sayfalı ve özet sayaçlarla birlikte getirir.
    Task<AdminUsersResponseDto> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize);

    // Admin panelinden kullanıcı hesabını aktif veya pasif yapar.
    Task<AdminUserDto> UpdateUserStatusAsync(int userId, AdminUpdateUserStatusDto dto, int currentAdminId);
    Task<AdminUserDto> UpdateUserRolesAsync(int userId, AdminUpdateUserRolesDto dto, int currentAdminId);

    // Admin paneli icin rezervasyon listesini filtreli ve sayfali sekilde getirir.
    Task<PagedResponseDto<ReservationDto>> GetReservationsAsync(string? status, int? hotelId, DateTime? from, DateTime? to, int page, int pageSize);

    // Admin panelinden rezervasyon durumunu (Confirmed/Cancelled/Completed) gunceller.
    Task<ReservationDto> UpdateReservationStatusAsync(int reservationId, string status);

    // Admin panelinden rezervasyonu onaylar (Confirmed) veya reddeder (Cancelled).
    Task<ReservationDto> ConfirmReservationAsync(int reservationId, bool confirm);

    // E44 - filtreli ve sayfalı log listesini sayım ile birlikte zarflayıp döndürür
    Task<PagedResponseDto<LogDto>> GetLogsAsync(LogFilterDto filter);

    // E45 - tek log kaydını detaylı şekilde getirir; bulunamazsa null döner
    Task<LogDto?> GetLogByIdAsync(int id);
}
