using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApp.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(HotelAppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByHotelIdAsync(int hotelId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .Where(x => x.HotelId == hotelId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Anasayfa vitrininde gosterilecek yorumlari puan ve tarihe gore siralar.
        public async Task<IEnumerable<Review>> GetFeaturedAsync(int count)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Review?> GetByUserAndHotelAsync(int userId, int hotelId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.UserId == userId && x.HotelId == hotelId);
        }

        public async Task<Review?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Admin yorum yonetim sayfasi icin sayfali liste. Once toplam sayi alinir,
        // sonra sayfa kadar kayit Include ile birlikte cekilir.
        public async Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedWithDetailsAsync(int page, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();

            var items = await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
