using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReviewDto>> GetHotelReviewAsync(int hotelId)
        {
            var review = await _unitOfWork.ReviewRepository.GetByHotelIdAsync(hotelId);

            return review.Select(MapToDto);
        }

        public async Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByUserIdAsync(userId);

            return reviews.Select(MapToDto);
        }

        // Anasayfa testimonial kartlari icin en yuksek puanli ve en yeni yorumlari hazirlar.
        public async Task<IEnumerable<ReviewDto>> GetFeaturedReviewsAsync(int count)
        {
            // Vitrin endpoint'inde asiri veri cekilmesini engeller.
            var safeCount = Math.Clamp(count, 1, 10);
            var reviews = await _unitOfWork.ReviewRepository.GetFeaturedAsync(safeCount);

            return reviews.Select(MapToDto);
        }

        public async Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new InvalidOperationException("Puan 1 ile 5 arasında olmalıdır.");

            var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(dto.HotelId);

            if (hotel == null)
                throw new KeyNotFoundException("Otel bulunamadı.");

            // Eligibility: Yorum yazabilmek icin kullanicinin bu otelde tamamlanmis
            // (veya checkout tarihi gecmis Confirmed) konaklamasi olmalidir.
            // Bu kontrol, rezervasyon yapmamis kullanicinin yorum eklemesini engeller.
            var hasStayed = await _unitOfWork.ReservationRepository
                .HasCompletedStayAsync(userId, dto.HotelId);

            if (!hasStayed)
                throw new InvalidOperationException(
                    "Bu otele yorum yazabilmek için tamamlanmış bir konaklamanız olmalıdır.");

            var existingReview = await _unitOfWork.ReviewRepository.GetByUserAndHotelAsync(userId, dto.HotelId);

            if (existingReview != null)
                throw new InvalidOperationException("Bu otele daha önce yorum yaptınız.");

            var review = new Review
            {
                HotelId = dto.HotelId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            var reviewWithDetails = await _unitOfWork.ReviewRepository.GetWithDetailsAsync(review.Id);

            if (reviewWithDetails == null)
                throw new InvalidOperationException("Yorum oluşturuldu fakat detayları geri okunamadı.");

            return MapToDto(reviewWithDetails);
        }

        public async Task DeleteReviewAsync(int id, int userId)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id);

            if (review == null)
                throw new KeyNotFoundException("Yorum bulunamadı.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz yok.");

            _unitOfWork.ReviewRepository.Delete(review);
            await _unitOfWork.SaveChangesAsync();
        }

        public static ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                HotelId = review.HotelId,
                UserId = review.UserId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UserName = $"{review.User?.FirstName} {review.User?.LastName}".Trim(),
                HotelName = review.Hotel?.Name ?? string.Empty
            };
        }

    }
}
