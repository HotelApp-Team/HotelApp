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
    /// <summary>
    /// Otel yorumlariyla ilgili is kurallarini yoneten servis sinifidir.
    /// Yorum listeleme, one cikan yorumlari getirme, yorum olusturma,
    /// yorum silme ve admin icin sayfali listeleme islemlerini gerceklestirir.
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// ReviewService sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="unitOfWork">Repository islemlerine erisim icin kullanilan UnitOfWork nesnesi.</param>
        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Belirtilen otele ait yorumlari getirir.
        /// </summary>
        /// <param name="hotelId">Yorumlari listelenecek otelin Id degeri.</param>
        /// <returns>Otele ait yorum DTO listesini doner.</returns>
        public async Task<IEnumerable<ReviewDto>> GetHotelReviewAsync(int hotelId)
        {
            var review = await _unitOfWork.ReviewRepository.GetByHotelIdAsync(hotelId);

            return review.Select(MapToDto);
        }

        /// <summary>
        /// Belirtilen kullaniciya ait yorumlari getirir.
        /// </summary>
        /// <param name="userId">Yorumlari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullaniciya ait yorum DTO listesini doner.</returns>
        public async Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByUserIdAsync(userId);

            return reviews.Select(MapToDto);
        }

        /// <summary>
        /// Anasayfa testimonial kartlari icin one cikan yorumlari getirir.
        /// Count degeri 1 ile 10 arasinda sinirlandirilir.
        /// </summary>
        /// <param name="count">Getirilecek yorum sayisi.</param>
        /// <returns>One cikan yorum DTO listesini doner.</returns>
        public async Task<IEnumerable<ReviewDto>> GetFeaturedReviewsAsync(int count)
        {
            // Vitrin endpoint'inde asiri veri cekilmesini engeller.
            var safeCount = Math.Clamp(count, 1, 10);
            var reviews = await _unitOfWork.ReviewRepository.GetFeaturedAsync(safeCount);

            return reviews.Select(MapToDto);
        }

        /// <summary>
        /// Belirtilen kullanici icin yeni otel yorumu olusturur.
        /// Puan araligini, otel varligini, konaklama gecmisini ve tekrar yorum kontrolunu yapar.
        /// </summary>
        /// <param name="userId">Yorumu olusturan kullanicinin Id degeri.</param>
        /// <param name="dto">Yorum olusturma bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan yorum DTO nesnesini doner.</returns>
        /// <exception cref="InvalidOperationException">Is kurali veya validasyon ihlali durumunda firlatilir.</exception>
        /// <exception cref="KeyNotFoundException">Otel bulunamazsa firlatilir.</exception>
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
                CreatedAt = DateTime.UtcNow,
                // Yeni yorumlar admin onayina kadar public alanlarda gosterilmez.
                IsApproved = false,
                ApprovedAt = null
            };

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            var reviewWithDetails = await _unitOfWork.ReviewRepository.GetWithDetailsAsync(review.Id);

            if (reviewWithDetails == null)
                throw new InvalidOperationException("Yorum oluşturuldu fakat detayları geri okunamadı.");

            return MapToDto(reviewWithDetails);
        }

        /// <summary>
        /// Belirtilen yorumu siler.
        /// Kullanici sadece kendi yorumunu silebilir; admin baskasina ait yorumlari da silebilir.
        /// </summary>
        /// <param name="id">Silinecek yorumun Id degeri.</param>
        /// <param name="userId">Islemi yapan kullanicinin Id degeri.</param>
        /// <param name="isAdmin">Islemi yapan kullanicinin admin olup olmadigini belirtir.</param>
        /// <exception cref="KeyNotFoundException">Yorum bulunamazsa firlatilir.</exception>
        /// <exception cref="UnauthorizedAccessException">Kullanicinin silme yetkisi yoksa firlatilir.</exception>
        public async Task DeleteReviewAsync(int id, int userId, bool isAdmin)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id);

            if (review == null)
                throw new KeyNotFoundException("Yorum bulunamadı.");

            // Yorumun sahibi degilse ve admin de degilse silme yetkisi yok.
            // ReservationService.CancelReservationAsync ile ayni patern.
            if (review.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz yok.");

            _unitOfWork.ReviewRepository.Delete(review);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Admin yorum yonetim sayfasi icin tum yorumlari sayfali olarak getirir.
        /// Page ve pageSize degerlerini guvenli araliklara ceker.
        /// </summary>
        /// <param name="page">Getirilecek sayfa numarasi.</param>
        /// <param name="pageSize">Sayfa basina getirilecek kayit sayisi.</param>
        /// <returns>Sayfalanmis yorum DTO sonucunu doner.</returns>
        public async Task<PagedResponseDto<ReviewDto>> GetAllReviewsAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var (reviews, totalCount) = await _unitOfWork.ReviewRepository
                .GetPagedWithDetailsAsync(page, pageSize);

            return new PagedResponseDto<ReviewDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = reviews.Select(MapToDto).ToList()
            };
        }

        /// <summary>
        /// Admin tarafindan belirtilen yorumu onaylayarak public alanlarda yayina alir.
        /// </summary>
        public async Task<ReviewDto> ApproveReviewAsync(int id)
        {
            var review = await _unitOfWork.ReviewRepository.GetForApprovalAsync(id);

            if (review == null)
                throw new KeyNotFoundException("Yorum bulunamadı.");

            // Daha once onaylanan yorum icin islemi tekrar calistirmak veri degistirmez.
            if (!review.IsApproved)
            {
                review.IsApproved = true;
                review.ApprovedAt = DateTime.UtcNow;

                _unitOfWork.ReviewRepository.Update(review);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToDto(review);
        }

        /// <summary>
        /// Review entity nesnesini ReviewDto nesnesine donusturur.
        /// </summary>
        /// <param name="review">Donusturulecek yorum entity nesnesi.</param>
        /// <returns>ReviewDto nesnesini doner.</returns>
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
                IsApproved = review.IsApproved,
                ApprovedAt = review.ApprovedAt,
                UserName = $"{review.User?.FirstName} {review.User?.LastName}".Trim(),
                HotelName = review.Hotel?.Name ?? string.Empty
            };
        }

    }
}
