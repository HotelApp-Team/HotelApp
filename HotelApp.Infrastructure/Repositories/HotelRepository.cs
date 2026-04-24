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
    public IQueryable<Hotel> Query()
    {
        return _context.Hotels
            .Where(x => x.IsActive)
            .Include(x => x.HotelImages)
            .AsNoTracking();
    }

    public async Task<(List<Hotel> Data, int TotalCount)> GetPagedFilteredAsync(string? city, byte? star, string? sortBy, string? sortOrder, int page, int pageSize)
    {
        var query = _context.Hotels
        .Include(x => x.HotelImages)
        .AsQueryable();

        if (!string.IsNullOrEmpty(city))
            query = query.Where(x => x.City.ToLower() == city.ToLower());

        if (star.HasValue)
            query = query.Where(x => x.StarRating == star);

        query = (sortBy?.ToLower(), sortOrder?.ToLower()) switch
        {
            ("star", "desc") => query.OrderByDescending(x => x.StarRating),
            ("star", _) => query.OrderBy(x => x.StarRating),

            ("rating", "desc") => query.OrderByDescending(x => x.AvgRating),
            ("rating", _) => query.OrderBy(x => x.AvgRating),

            (_, "desc") => query.OrderByDescending(x => x.Name),
            _ => query.OrderBy(x => x.Name)
        };

        var totalCount = await query.CountAsync();

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (data, totalCount);
    }
}
