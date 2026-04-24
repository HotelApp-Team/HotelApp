using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

public class HotelRepository : Repository<Hotel>, IHotelRepository
{
    public HotelRepository(HotelAppDbContext context) : base(context)
    {
    }

    public async Task<Hotel?> GetWithDetailsAsync(int id)
    {
        return await _context.Hotels
            .Include(x => x.HotelImages)
            .Include(x => x.HotelRooms)
            .Include(x => x.Reviews)
            .Include(x => x.Amenities)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Hotel>> GetByCityAsync(string city)
    {
        return await _context.Hotels
            .Include(x => x.HotelImages)
            .Where(x => x.City.ToLower() == city.ToLower())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Hotel>> GetActiveAsync()
    {
        return await _context.Hotels
            .Where(x => x.IsActive)
            .Include(x => x.HotelImages)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<string>> GetActiveCitiesAsync()
    {
        return await _context.Hotels
            .Where(x => x.IsActive)
            .Select(x => x.City)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }
}
