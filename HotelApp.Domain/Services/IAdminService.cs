using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardDataAsync();
    Task<AdminUsersResponseDto> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize);
    Task<AdminUserDto> UpdateUserStatusAsync(int userId, AdminUpdateUserStatusDto dto, int currentAdminId);
    Task<AdminUserDto> UpdateUserRolesAsync(int userId, AdminUpdateUserRolesDto dto, int currentAdminId);

    Task<PagedResponseDto<ReservationDto>> GetReservationsAsync(string? status, int? hotelId, DateTime? from, DateTime? to, int page, int pageSize);
    Task<ReservationDto> UpdateReservationStatusAsync(int reservationId, string status);
    Task<ReservationDto> ConfirmReservationAsync(int reservationId, bool confirm);

    Task<PagedResponseDto<LogDto>> GetLogsAsync(LogFilterDto filter);
    Task<LogDto?> GetLogByIdAsync(int id);
}
