using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

public interface IHotelImageRepository : IRepository<HotelImage>
{
    // Belirli bir otele ait tüm resimleri getir
    Task<IEnumerable<HotelImage>> GetByHotelIdAsync(int hotelId);

    // Ana resmi getir (IsMain = true)
    Task<HotelImage?> GetMainImageAsync(int hotelId);

    // Ana resmi değiştir (tek main garanti eder)
    Task SetMainImageAsync(int hotelId, int imageId);
    Task<HotelImage?> GetByHotelAndImageIdAsync(int hotelId, int imageId);
}
