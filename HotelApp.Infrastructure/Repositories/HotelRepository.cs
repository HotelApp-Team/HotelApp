using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

// Otel verilerine ait �zel sorgular� y�neten repository s�n�f�d�r.
// Listeleme, detay getirme, filtreleme ve m�sait otel arama i�lemlerini yapar
public class HotelRepository : Repository<Hotel>, IHotelRepository
{
    // DbContext base Repository s�n�f�na g�nderilir.
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

    // Id de�erine g�re oteli ili�kili verileriyle birlikte getirir.
    public async Task<Hotel?> GetWithDetailsAsync(int id)
    {
        return await _context.Hotels
            .Include(x => x.HotelImages)
            .Include(x => x.HotelRooms)
            .Include(x => x.Reviews)
            .Include(x => x.Amenities)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // Verilen �ehir ad�na g�re otelleri getirir.
    // Liste ekran�nda kapak g�rseli i�in HotelImages da dahil edilir.
    public async Task<IEnumerable<Hotel>> GetByCityAsync(string city)
    {
        return await _context.Hotels
            .Include(x => x.HotelImages)
            .Where(x => x.City.ToLower() == city.ToLower())
            .AsNoTracking()
            .ToListAsync();
    }

    // Sadece aktif durumdaki otelleri getirir.
    public async Task<IEnumerable<Hotel>> GetActiveAsync()
    {
        return await _context.Hotels
            .Where(x => x.IsActive)
            .Include(x => x.HotelImages)
            .AsNoTracking()
            .ToListAsync();
    }

    // Aktif otellerin bulundu�u �ehirleri tekil olarak getirir.
    public async Task<List<string>> GetActiveCitiesAsync()
    {
        return await _context.Hotels
            .Where(x => x.IsActive)
            .Select(x => x.City)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }

    // Hero istatistigi icin aktif otel sayisini sadece COUNT sorgusu ile hesaplar.
    public async Task<int> GetActiveHotelCountAsync()
    {
        return await _context.Hotels
            .AsNoTracking()
            .CountAsync(x => x.IsActive);
    }

    // Hero istatistigi icin aktif otellerin bulundugu farkli sehir sayisini hesaplar.
    public async Task<int> GetActiveCityCountAsync()
    {
        return await _context.Hotels
            .AsNoTracking()
            .Where(x => x.IsActive && x.City != null && x.City.Trim() != "")
            .Select(x => x.City.Trim())
            .Distinct()
            .CountAsync();
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

    // Aktif oteller i�in temel IQueryable sorgusu olu�turur.
    public IQueryable<Hotel> Query()
    {
        return _context.Hotels
            .Where(x => x.IsActive)
            .Include(x => x.HotelImages)
            .AsNoTracking();
    }

    // Admin otel listesi i�in filtreleme, s�ralama ve sayfalama yapar.
    // Geriye hem ilgili sayfadaki veriyi hem de toplam kay�t say�s�n� d�nd�r�r.
    public async Task<(List<Hotel> Data, int TotalCount)> GetPagedFilteredAsync(string? city, byte? star, bool? isActive, int? hotelId, string? sortBy, string? sortOrder, int page, int pageSize)
    {
        // Admin paneli aktif ve pasif otelleri yonetebildigi icin taban sorgu
        // tum otelleri kapsar; aktiflik yalnizca isActive filtresi geldiginde uygulanir.
        var query = _context.Hotels
            .Include(x => x.HotelImages)
            .AsQueryable();

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        if (!string.IsNullOrEmpty(city))
            query = query.Where(x => x.City.ToLower() == city.ToLower());

        if (hotelId.HasValue)
            query = query.Where(x => x.Id == hotelId.Value);

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

    // Arama kriterlerine g�re m�sait otelleri sayfal� olarak getirir.
    // �nce stored procedure ile m�sait odalar bulunur, sonra bu oteller filtrelenir.
    public async Task<(List<HotelListDto> Data, int TotalCount)> SearchAvailableHotelsPagedAsync(string city, DateTime checkIn, DateTime checkOut, int guestCount, byte? starRating, decimal? minRating, int page, int pageSize)
    {
        // Stored procedure ile m�sait oda bilgileri al�n�r.
        var availableRooms = await _context.Database
        .SqlQueryRaw<AvailableRoomDto>(
            "EXEC sp_SearchAvailableRooms @HotelId = @HotelId, @City = @City, @CheckInDate = @CheckInDate, @CheckOutDate = @CheckOutDate, @GuestCount = @GuestCount",
            new SqlParameter("@HotelId", DBNull.Value),
            new SqlParameter("@City", city),
            new SqlParameter("@CheckInDate", checkIn.Date),
            new SqlParameter("@CheckOutDate", checkOut.Date),
            new SqlParameter("@GuestCount", guestCount))
        .ToListAsync();

        // M�sait odalara sahip otel Id de�erleri tekille�tirilir.
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


    // Navbar aramas�ndan gelen ifadeyi otel ad� ve �ehir alanlar�nda arar.
    // Yaln�zca aktif otelleri getirir ve sonucu sayfal� olarak d�nd�r�r.
    public async Task<(List<Hotel> Data, int TotalCount)> SearchCatalogPagedAsync(
        string searchTerm,
        int page,
        int pageSize)
    {
        // Kullan�c�n�n girdi�i ba�taki ve sondaki bo�luklar� temizler.
        var term = searchTerm.Trim();

        // Otel kartlar�nda kullan�lacak g�rsel ve oda bilgilerini dahil eder.
        var query = _context.Hotels
            .AsNoTracking()
            .Include(h => h.HotelImages)
            .Include(h => h.HotelRooms)
            .Where(h => h.IsActive &&
            (EF.Functions.Like(h.Name, $"%{term}%") ||
             EF.Functions.Like(h.City, $"%{term}%")));

        // Sayfalama yap�lmadan �nce toplam sonu� say�s�n� hesaplar.
        var totalCount = await query.CountAsync();

        //�stenen sayfadaki otelleri alfabetik s�rayla getirir.
        var hotels = await query
                        .OrderBy(h => h.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (hotels, totalCount);
    }
}
