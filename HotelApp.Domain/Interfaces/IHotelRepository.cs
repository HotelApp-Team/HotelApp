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
}
