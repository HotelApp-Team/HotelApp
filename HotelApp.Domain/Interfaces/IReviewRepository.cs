using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByHotelIdAsync(int hotelId);
        Task<IEnumerable<Review>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Review>> GetFeaturedAsync(int count);
        Task<Review?> GetByUserAndHotelAsync(int userId, int hotelId);
        Task<Review?> GetWithDetailsAsync(int id);
    }
}
