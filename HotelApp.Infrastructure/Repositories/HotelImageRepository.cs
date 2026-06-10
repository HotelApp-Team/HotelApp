using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

// Otel görsellerine ait özel veritabanı sorgularını yönetir.
// Görselleri otele göre listeleme, ana görsel getirme ve ana görsel değiştirme işlemlerini yapar.
public class HotelImageRepository : Repository<HotelImage>, IHotelImageRepository
{
    // Repository sınıfına veritabanı context bilgisini gönderir.
    public HotelImageRepository(HotelAppDbContext context) : base(context)
    {
    }

    // Belirtilen otel Id ve görsel Id değerlerine göre tek bir görsel kaydı getirir.
    // Görselin doğru otele ait olduğunu doğrulamak için kullanılır.
    public async Task<HotelImage?> GetByHotelAndImageIdAsync(int hotelId, int imageId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.HotelId == hotelId && x.Id == imageId);
    }

    // Belirtilen otele ait tüm görselleri getirir.
    // DisplayOrder değerine göre sıralı döndürür.
    public async Task<IEnumerable<HotelImage>> GetByHotelIdAsync(int hotelId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.HotelId == hotelId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    // Belirtilen otelin ana görselini getirir.
    // IsMain değeri true olan ilk görsel döndürülür.
    public async Task<HotelImage?> GetMainImageAsync(int hotelId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.HotelId == hotelId && x.IsMain);
    }

    // Belirtilen görseli otelin ana görseli yapar.
    // Önce görselin varlığını ve ilgili otele ait olup olmadığını kontrol eder.
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

        // Aynı otele ait görsellerden sadece seçilen görsel ana görsel yapılır.
        foreach (var img in images)
        {
            img.IsMain = img.Id == imageId;
        }
    }
}
