using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

public class HotelRepository : Repository<Hotel>, IHotelRepository
{
    public HotelRepository(HotelAppDbContext context) : base(context)
    {
    }

    // Liste cagirimlarinda kapak resmi (MainImageUrl) bos gelmesin diye
    // HotelImages'i include ederek donuyoruz.
    public override async Task<IEnumerable<Hotel>> GetAllAsync()
    {
        return await _context.Hotels
            .Include(x => x.HotelImages)
            .AsNoTracking()
            .ToListAsync();
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

    // Anasayfa sehir kartlari icin gruplama ve siralama sorgusunu DB tarafinda yapar.
    public async Task<List<CityCardDto>> GetFeaturedCityStatsAsync(int count)
    {
        return await _context.Hotels
            .Where(x => x.IsActive && x.City != null && x.City.Trim() != "")
            .GroupBy(x => x.City.Trim())
            .Select(g => new
            {
                Name = g.Key,
                HotelCount = g.Count(),
                AverageRating = g.Average(x => x.AvgRating ?? 0)
            })
            .OrderByDescending(x => x.HotelCount)
            .ThenByDescending(x => x.AverageRating)
            .ThenBy(x => x.Name)
            .Take(count)
            .Select(city => new CityCardDto
            {
                Name = city.Name,
                HotelCount = city.HotelCount
            })
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
            .Where(x => x.IsActive)
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

    public async Task<(List<HotelListDto> Data, int TotalCount)> SearchAvailableHotelsPagedAsync(string city, DateTime checkIn, DateTime checkOut, int guestCount, byte? starRating, decimal? minRating, int page, int pageSize)
    {
        var availableRooms = await _context.Database
        .SqlQueryRaw<AvailableRoomDto>(
            "EXEC sp_SearchAvailableRooms @HotelId = @HotelId, @City = @City, @CheckInDate = @CheckInDate, @CheckOutDate = @CheckOutDate, @GuestCount = @GuestCount",
            new SqlParameter("@HotelId", DBNull.Value),
            new SqlParameter("@City", city),
            new SqlParameter("@CheckInDate", checkIn.Date),
            new SqlParameter("@CheckOutDate", checkOut.Date),
            new SqlParameter("@GuestCount", guestCount))
        .ToListAsync();

        var availableHotelIds = availableRooms
            .Select(x => x.HotelId)
            .Distinct()
            .ToList();

        var query = _dbSet
            .Include(h => h.HotelImages)
            .Where(h => h.IsActive && availableHotelIds.Contains(h.Id));

        if (starRating.HasValue)
            query = query.Where(h => h.StarRating == starRating.Value);

        if (minRating.HasValue)
            query = query.Where(h => h.AvgRating.HasValue && h.AvgRating.Value >= minRating.Value);

        var totalCount = await query.CountAsync();

        var data = await query
            .OrderBy(h => h.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(h => new HotelListDto
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Country = h.Country,
                StarRating = h.StarRating,
                AvgRating = h.AvgRating,
                IsActive = h.IsActive,
                MainImageUrl = h.HotelImages
                    .Where(i => i.IsMain)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return (data, totalCount);
    }
}
