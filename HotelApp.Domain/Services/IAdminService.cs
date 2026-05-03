using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

// Amaç: Admin paneli iş kurallarının servis sözleşmesini tanımlar.
// SOLID: ISP ile sadece admin işlemlerini içerir; DIP ile controller somut sınıfa bağlı kalmaz.

public interface IAdminService
{
    // Dashboard için gerekli tüm istatistikleri döndürür.
    Task<AdminDashboardDto> GetDashboardDataAsync();

    // Admin paneli için kullanıcıları filtreli ve sayfalı şekilde getirir.
    Task<PagedResponseDto<AdminUserDto>> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize);

    // Admin panelinden kullanıcı hesabını aktif veya pasif yapar.
    Task<AdminUserDto> UpdateUserStatusAsync(int userId, AdminUpdateUserStatusDto dto, int currentAdminId);
}
