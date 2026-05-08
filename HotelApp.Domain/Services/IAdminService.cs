using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

// IAdminService arayüzünün amacı, admin paneli iş kuralları için servis sözleşmesi tanımlamaktır.
// Dashboard, kullanıcı listeleme, kullanıcı durumu ve rol güncelleme metotlarını belirler.
// SOLID: ISP kullanır; sadece admin servis ihtiyaçlarını içerir.
// SOLID: DIP sağlar; controller somut AdminService yerine bu arayüze bağlı kalır.

public interface IAdminService
{
    // Dashboard için gerekli tüm istatistikleri döndürür.
    Task<AdminDashboardDto> GetDashboardDataAsync();

    // Admin paneli için kullanıcıları filtreli ve sayfalı şekilde getirir.
    Task<PagedResponseDto<AdminUserDto>> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize);

    // Admin panelinden kullanıcı hesabını aktif veya pasif yapar.
    Task<AdminUserDto> UpdateUserStatusAsync(int userId, AdminUpdateUserStatusDto dto, int currentAdminId);

    // Admin panelinden kullanıcı rollerini günceller.
    Task<AdminUserDto> UpdateUserRolesAsync(int userId, AdminUpdateUserRolesDto dto, int currentAdminId);

    // E44 - filtreli ve sayfalı log listesini sayım ile birlikte zarflayıp döndürür
    Task<PagedResponseDto<LogDto>> GetLogsAsync(LogFilterDto filter);

    // E45 - tek log kaydını detaylı şekilde getirir; bulunamazsa null döner
    Task<LogDto?> GetLogByIdAsync(int id);

}
