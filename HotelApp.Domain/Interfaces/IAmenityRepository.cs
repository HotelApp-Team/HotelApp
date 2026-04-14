using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

public interface IAmenityRepository : IRepository<Amenity>
{
    // Otele ait olanakları getir
    Task<List<Amenity>> GetAmenitiesByHotelIdAsync(int hotelId);

    // Otele olanak ekle
    Task AddAmenityToHotelAsync(int hotelId, int amenityId);

    // Otelden olanak kaldır
    Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId);
}
