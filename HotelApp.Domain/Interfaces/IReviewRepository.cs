using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByHotelIdAsync(int hotelId);
        Task<Review?> GetByUserAndHotelAsync(int userId, int hotelId);
    }
}
