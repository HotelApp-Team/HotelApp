using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

// Amaç: Admin dashboard iş kurallarını servis sözleşmesi olarak tanımlar.
// SOLID: ISP ile sadece admin servis ihtiyacını içerir; DIP ile controller somut sınıfa bağlı kalmaz.

public interface IAdminService
{
    // Dashboard için gerekli tüm istatistikleri döndürür.
    Task<AdminDashboardDto> GetDashboardDataAsync();
}
