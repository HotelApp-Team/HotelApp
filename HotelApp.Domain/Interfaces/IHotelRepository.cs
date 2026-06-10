using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

// Hotel entity'sine özel repository metotlarını tanımlar.
// Genel CRUD işlemleri IRepository<Hotel> üzerinden gelir.
public interface IHotelRepository : IRepository<Hotel>
{
    // Detaylı otel (Include'lu)
    // Otelin görsellerini, odalarını, yorumlarını ve olanaklarını birlikte getirir.
    Task<Hotel?> GetWithDetailsAsync(int id);

    // Şehre göre filtreleme
    // Belirtilen şehirdeki otelleri listelemek için kullanılır.
    Task<IEnumerable<Hotel>> GetByCityAsync(string city);

    // Aktif oteller
    // Kullanıcı tarafında sadece yayında olan otelleri göstermek için kullanılır.
    Task<IEnumerable<Hotel>> GetActiveAsync();

    // Aktif otellerin bulunduğu şehirleri tekil olarak getirir.
    Task<List<string>> GetActiveCitiesAsync();

    // Anasayfa hero alaninda gosterilecek aktif otel sayisini DB tarafinda hesaplar.
    Task<int> GetActiveHotelCountAsync();

    // Anasayfa hero alaninda gosterilecek aktif sehir sayisini DB tarafinda hesaplar.
    Task<int> GetActiveCityCountAsync();

    // Anasayfa vitrinindeki sehir kartlari icin DB tarafinda gruplanmis veriyi getirir.
    Task<List<CityCardDto>> GetFeaturedCityStatsAsync(int count);

    // Aktif oteller için temel sorguyu döndürür.
    // Servis katmanı gerekirse bu sorguya ek filtreler uygulayabilir.
    IQueryable<Hotel> Query();

    // Admin otel listesi için filtreleme, sıralama ve sayfalama yapar.
    // Toplam kayıt sayısı ile birlikte ilgili sayfadaki otelleri döndürür.
    Task<(List<Hotel> Data, int TotalCount)> GetPagedFilteredAsync(string? city, byte? star, bool? isActive, string? sortBy, string? sortOrder, int page, int pageSize);

    // Otel arama ekranında müsait otelleri sayfalı olarak getirir.
    // Tarih, şehir ve misafir sayısı bilgilerine göre arama yapılır.
    Task<(List<HotelListDto> Data, int TotalCount)> SearchAvailableHotelsPagedAsync(string city, DateTime checkIn, DateTime checkOut, int guestCount, byte? starRating, decimal? minRating, int page, int pageSize);

}
