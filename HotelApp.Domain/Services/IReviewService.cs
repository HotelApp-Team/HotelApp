using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetHotelReviewAsync(int hotelId);
        Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId);
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto);
        Task DeleteReviewAsync(int id, int userId);
    }
}
