using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Infrastructure.Repositories
{
    public class HotelImageRepository : IHotelImageRepository
    {
        private readonly HotelAppDbContext _context;

        // 🔹 Clean kullanım için DbSet property
        private DbSet<HotelImage> Images => _context.HotelImages;

        public HotelImageRepository(HotelAppDbContext context)
        {
            _context = context;
        }

        // Belirli bir otele ait tüm resimler
        public async Task<IEnumerable<HotelImage>> GetByHotelIdAsync(int hotelId)
        {
            return await Images
                .AsNoTracking()
                .Where(x => x.HotelId == hotelId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        // Tek resim getir
        public async Task<HotelImage?> GetByIdAsync(int id)
        {
            return await Images
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Ana resmi getir
        public async Task<HotelImage?> GetMainImageAsync(int hotelId)
        {
            return await Images
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HotelId == hotelId && x.IsMain);
        }

        // Yeni resim ekle
        public async Task AddAsync(HotelImage image)
        {
            await Images.AddAsync(image);
        }

        // Güncelle
        public void Update(HotelImage image)
        {
            Images.Update(image);
        }

        // Sil
        public void Delete(HotelImage image)
        {
            Images.Remove(image);
        }

        // Ana resmi değiştir (tek main garanti eder)
        public async Task SetMainImageAsync(int hotelId, int imageId)
        {
            var images = await Images
                .Where(x => x.HotelId == hotelId)
                .ToListAsync();

            foreach (var img in images)
            {
                img.IsMain = img.Id == imageId;
            }
        }
    }
}
