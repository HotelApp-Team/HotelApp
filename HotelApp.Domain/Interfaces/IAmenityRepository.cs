using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

// Olanak repository için özel sorgu ve ilişki metotlarını tanımlar.
// Genel CRUD işlemleri IRepository<Amenity> üzerinden gelir.
public interface IAmenityRepository : IRepository<Amenity>
{
    // Otele ait olanakları getir.
    // Otel detay ve admin düzenleme ekranında kullanılır.
    Task<List<Amenity>> GetAmenitiesByHotelIdAsync(int hotelId);

    // Otele olanak ekle.
    // HotelAmenities ilişki tablosuna yeni bağlantı oluşturur.
    Task AddAmenityToHotelAsync(int hotelId, int amenityId);

    // Otelden olanak kaldır.
    // Otel ile olanak arasındaki ilişkiyi siler.
    Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId);
}
