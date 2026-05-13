using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

public interface IHotelRepository : IRepository<Hotel>
{
    // Detaylı otel (Include'lu)
    Task<Hotel?> GetWithDetailsAsync(int id);

    // Şehre göre filtreleme
    Task<IEnumerable<Hotel>> GetByCityAsync(string city);

    // Aktif oteller
    Task<IEnumerable<Hotel>> GetActiveAsync();
    Task<List<string>> GetActiveCitiesAsync();

    // Anasayfa hero alaninda gosterilecek aktif otel sayisini DB tarafinda hesaplar.
    Task<int> GetActiveHotelCountAsync();

    // Anasayfa hero alaninda gosterilecek aktif sehir sayisini DB tarafinda hesaplar.
    Task<int> GetActiveCityCountAsync();

    // Anasayfa vitrinindeki sehir kartlari icin DB tarafinda gruplanmis veriyi getirir.
    Task<List<CityCardDto>> GetFeaturedCityStatsAsync(int count);

    IQueryable<Hotel> Query();
    Task<(List<Hotel> Data, int TotalCount)> GetPagedFilteredAsync(string? city, byte? star, bool? isActive, string? sortBy, string? sortOrder, int page, int pageSize);
    Task<(List<HotelListDto> Data, int TotalCount)> SearchAvailableHotelsPagedAsync(string city, DateTime checkIn, DateTime checkOut, int guestCount, byte? starRating, decimal? minRating, int page, int pageSize);

}
