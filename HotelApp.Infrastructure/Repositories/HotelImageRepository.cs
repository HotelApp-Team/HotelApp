using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

public class HotelImageRepository : Repository<HotelImage>, IHotelImageRepository
{
    public HotelImageRepository(HotelAppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HotelImage>> GetByHotelIdAsync(int hotelId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.HotelId == hotelId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<HotelImage?> GetMainImageAsync(int hotelId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.HotelId == hotelId && x.IsMain);
    }

    public async Task SetMainImageAsync(int hotelId, int imageId)
    {
        var targetImage = await _dbSet.FirstOrDefaultAsync(x => x.Id == imageId);

        if (targetImage == null)
        {
            throw new InvalidOperationException("Resim bulunamadı.");
        }

        if (targetImage.HotelId != hotelId)
        {
            throw new InvalidOperationException("Seçilen resim bu otele ait değil.");
        }

        var images = await _dbSet
            .Where(x => x.HotelId == hotelId)
            .ToListAsync();

        foreach (var img in images)
        {
            img.IsMain = img.Id == imageId;
        }
    }
}
