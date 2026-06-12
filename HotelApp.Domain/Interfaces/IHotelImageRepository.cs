using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

// Otel görselleri için özel repository metotlarını tanımlar.
// Genel CRUD işlemleri IRepository<HotelImage> üzerinden gelir.
public interface IHotelImageRepository : IRepository<HotelImage>
{
    /// Belirli bir otele ait tüm resimleri getir.
    // Otel detay ve admin görsel listesi için kullanılır.
    Task<IEnumerable<HotelImage>> GetByHotelIdAsync(int hotelId);

    // Toplu güncelleme ve silme işlemleri için otele ait görselleri change tracking açık şekilde getirir.
    Task<List<HotelImage>> GetTrackedByHotelIdAsync(int hotelId);

    // Ana resmi getir (IsMain = true).
    // Otel kartında veya detay sayfasında kapak görseli olarak kullanılır.
    Task<HotelImage?> GetMainImageAsync(int hotelId);

    // Ana resmi değiştir (tek main garanti eder).
    // Aynı otele ait diğer görsellerin IsMain değerini false yapar.
    Task SetMainImageAsync(int hotelId, int imageId);

    // Belirtilen otel ve görsel Id değerlerine göre görseli getirir.
    // Görselin gerçekten ilgili otele ait olup olmadığını kontrol etmek için kullanılır.
    Task<HotelImage?> GetByHotelAndImageIdAsync(int hotelId, int imageId);
}
