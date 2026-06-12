using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

// Olanak verilerine ait özel veritabanı işlemlerini yönetir.
// Otel ile olanak arasındaki ekleme, listeleme ve kaldırma işlemlerini yapar.
public class AmenityRepository : Repository<Amenity>, IAmenityRepository
{
    // Repository sınıfına veritabanı context bilgisini gönderir.
    public AmenityRepository(HotelAppDbContext context) : base(context)
    {
    }

    // Belirtilen otele atanmış olanakları getirir.
    // Hotels ile Amenities ilişkisi üzerinden sorgu yapılır.
    public async Task<List<Amenity>> GetAmenitiesByHotelIdAsync(int hotelId)
    {
        return await _context.Hotels
            .Where(h => h.Id == hotelId)
            .SelectMany(h => h.Amenities)
            .AsNoTracking()
            .ToListAsync();
    }

    // Belirtilen otele yeni bir olanak ekler.
    // Önce otelin ve olanağın var olup olmadığını kontrol eder.
    public async Task AddAmenityToHotelAsync(int hotelId, int amenityId)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == hotelId);

        if (hotel == null)
        {
            throw new InvalidOperationException("Otel bulunamadı.");
        }

        var amenity = await _context.Amenities.FindAsync(amenityId);

        if (amenity == null)
        {
            throw new InvalidOperationException("Olanak bulunamadı.");
        }

        if (hotel.Amenities.Any(a => a.Id == amenityId))
        {
            throw new InvalidOperationException("Bu olanak otele zaten eklenmiş.");
        }

        hotel.Amenities.Add(amenity);
    }

    // Belirtilen otelden seçilen olanağı kaldırır.
    // Otel bulunamazsa hata fırlatır, olanak ilişkisi varsa siler.
    public async Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == hotelId);

        if (hotel == null)
        {
            throw new InvalidOperationException("Otel bulunamadı.");
        }

        var amenity = hotel.Amenities.FirstOrDefault(a => a.Id == amenityId);

        if (amenity != null)
        {
            hotel.Amenities.Remove(amenity);
        }
    }
}
