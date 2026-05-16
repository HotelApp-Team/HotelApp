using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

// IAdminRepository arayüzünün amacı, admin paneli için gereken veri erişim sözleşmesini tanımlamaktır.
// Dashboard, kullanıcı listeleme, kullanıcı bulma ve rol sorgulama metotlarını belirler.
// SOLID: ISP kullanır; sadece admin repository ihtiyaçlarını içerir.
// SOLID: DIP sağlar; üst katmanlar somut repository yerine bu arayüze bağlı kalır.

public interface IAdminRepository
{
    // Toplam otel sayısını döndürür.
    Task<int> GetTotalHotelsAsync();

    // Toplam kullanıcı sayısını döndürür.
    Task<int> GetTotalUsersAsync();

    // Toplam rezervasyon sayısını döndürür.
    Task<int> GetTotalReservationsAsync();

    // Onaylı rezervasyon sayısını döndürür.
    Task<int> GetActiveReservationsAsync();

    // Tamamlanmış rezervasyonlardan toplam geliri hesaplar.
    Task<decimal> GetTotalRevenueAsync();

    // Otellerin ortalama puanını hesaplar.
    Task<decimal> GetAverageRatingAsync();

    // En son oluşturulan rezervasyonları listeler.
    Task<List<Reservation>> GetRecentReservationsAsync(int count);

    // Admin kullanıcı listesi için arama, aktiflik filtresi ve sayfalama sorgusunu yapar.
    Task<(List<User> Data, int TotalCount)> GetPagedUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize);

    // Admin kullanıcı özet kartları için sayfalamadan bağımsız toplamları döndürür.
    Task<(int ActiveCount, int PassiveCount, int AdminCount)> GetUserSummaryForAdminAsync(string? search);

    // Admin işlemleri için kullanıcıyı rolleriyle birlikte getirir.
    Task<User?> GetUserByIdAsync(int userId);

    // Role isimlerine göre rolleri getirir.
    Task<List<Role>> GetRolesByNamesAsync(List<string> roleNames);

    // Admin rezervasyon ekranı için filtreli ve sayfalı rezervasyon listesi döndürür.
    Task<(List<Reservation> Data, int TotalCount)> GetPagedReservationsForAdminAsync(string? status, int? hotelId, DateTime? from, DateTime? to, int page, int pageSize);

    // Admin işlemlerinde değişen kullanıcıyı güncellenecek olarak işaretler.
    void UpdateUser(User user);
}
