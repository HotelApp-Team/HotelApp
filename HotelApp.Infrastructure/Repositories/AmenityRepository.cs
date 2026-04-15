using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

public class AmenityRepository : Repository<Amenity>, IAmenityRepository
{
    public AmenityRepository(HotelAppDbContext context) : base(context)
    {
    }

    public async Task<List<Amenity>> GetAmenitiesByHotelIdAsync(int hotelId)
    {
        return await _context.Hotels
            .Where(h => h.Id == hotelId)
            .SelectMany(h => h.Amenities)
            .AsNoTracking()
            .ToListAsync();
    }

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
