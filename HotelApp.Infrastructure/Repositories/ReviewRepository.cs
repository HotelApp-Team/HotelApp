using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(HotelAppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetByHotelIdAsync(int hotelId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Where(x => x.HotelId == hotelId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetByUserAndHotelAsync(int userId, int hotelId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.UserId == userId && x.HotelId == hotelId);
        }
    }
}
