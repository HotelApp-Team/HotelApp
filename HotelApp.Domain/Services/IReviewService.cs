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
        Task<IEnumerable<ReviewDto>> GetFeaturedReviewsAsync(int count);
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto);

        // Yorum silme: kullanici sadece kendi yorumunu siler, admin baskasininkini de silebilir.
        Task DeleteReviewAsync(int id, int userId, bool isAdmin);

        // Admin yorum yonetim ekrani icin sayfali tum yorum listesi.
        Task<PagedResponseDto<ReviewDto>> GetAllReviewsAsync(int page, int pageSize);
    }
}
