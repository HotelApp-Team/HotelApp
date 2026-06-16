using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services
{
    // Sehir (il) listesini sunan service sozlesmesi.
    // Seed-only lookup oldugu icin CRUD yok; sadece okuma yapilir.
    public interface ICityService
    {
        // Tum sehirleri ada gore sirali getirir (form dropdown'i icin).
        Task<IEnumerable<CityDto>> GetAllAsync();
    }
}
