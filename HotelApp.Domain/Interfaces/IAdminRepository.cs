using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

// Amaç: Admin dashboard için gereken veritabanı sorgularının sözleşmesini belirler.
// SOLID: ISP ile sadece admin sorgularını içerir; DIP ile üst katmanlar somut repository yerine bu arayüze bağlı kalır.

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
}
