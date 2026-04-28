using HotelApp.Domain.DTOs;
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
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto);
        Task DeleteReviewAsync(int id, int userId);
    }
}
