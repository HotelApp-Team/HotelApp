using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers
{
    /// <summary>
    /// Otel degerlendirmelerinin listelenmesi, olusturulmasi,
    /// silinmesi ve admin tarafindan yonetilmesi islemlerini yonetir.
    /// </summary>
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {

        private readonly IReviewService _reviewService;
        private readonly ILoggerService _logger;

        /// <summary>
        /// ReviewController sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="reviewService">Degerlendirme islemleri icin kullanilan servis.</param>
        /// <param name="logger">Loglama islemleri icin kullanilan servis.</param>
        public ReviewController(IReviewService reviewService, ILoggerService logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        /// <summary>
        /// Anasayfada gosterilecek one cikan degerlendirmeleri getirir.
        /// </summary>
        /// <param name="count">Getirilecek one cikan degerlendirme sayisi.</param>
        /// <returns>One cikan degerlendirme listesini doner.</returns>
        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedReviews([FromQuery] int count = 3)
        {
            var reviews = await _reviewService.GetFeaturedReviewsAsync(count);

            return Ok(reviews);
        }

        /// <summary>
        /// Belirtilen otele ait degerlendirmeleri getirir.
        /// </summary>
        /// <param name="hotelId">Degerlendirmeleri listelenecek otelin Id degeri.</param>
        /// <returns>Otele ait degerlendirme listesini doner.</returns>
        [AllowAnonymous]
        [HttpGet("~/api/hotels/{hotelId}/reviews")]
        public async Task<IActionResult> GetHotelReviews(int hotelId)
        {
            var review = await _reviewService.GetHotelReviewAsync(hotelId);
            
            return Ok(review);
        }

        /// <summary>
        /// Admin paneli icin tum degerlendirmeleri sayfali olarak getirir.
        /// </summary>
        /// <param name="page">Getirilecek sayfa numarasi.</param>
        /// <param name="pageSize">Sayfa basina getirilecek kayit sayisi.</param>
        /// <returns>Sayfalanmis degerlendirme sonucunu doner.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("~/api/admin/reviews")]
        public async Task<IActionResult> GetAllForAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _reviewService.GetAllReviewsAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Admin tarafindan bekleyen bir yorumu onaylayarak yayina alir.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("~/api/admin/reviews/{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var review = await _reviewService.ApproveReviewAsync(id);

            await LogInfoAsync($"Yorum onaylandi: ReviewId={id}");

            return Ok(review);
        }

        /// <summary>
        /// Giris yapmis kullanicinin kendi degerlendirmelerini getirir.
        /// </summary>
        /// <returns>Kullaniciya ait degerlendirme listesini doner.</returns>
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyReviews()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var review = await _reviewService.GetUserReviewsAsync(userId);

            return Ok(review);
        }

        /// <summary>
        /// Giris yapmis kullanici icin yeni degerlendirme olusturur.
        /// </summary>
        /// <param name="dto">Olusturulacak degerlendirme bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan degerlendirme kaydini doner.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var review = await _reviewService.CreateReviewAsync(userId, dto);

            await LogInfoAsync(
                $"Yeni değerlendirme eklendi: ReviewId={review.Id}, HotelId={dto.HotelId}",
                userId: userId);

            return Created(string.Empty, review);
        }

        /// <summary>
        /// Belirtilen degerlendirmeyi siler.
        /// Kullanici kendi degerlendirmesini, admin ise tum degerlendirmeleri silebilir.
        /// </summary>
        /// <param name="id">Silinecek degerlendirmenin Id degeri.</param>
        /// <returns>Basarili olursa 204 NoContent doner.</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            await _reviewService.DeleteReviewAsync(id, userId, isAdmin);

            await LogInfoAsync(
                $"Değerlendirme silindi: ReviewId={id}, isAdmin={isAdmin}",
                userId: userId);

            return NoContent();
        }

        /// <summary>
        /// JWT claim bilgileri uzerinden giris yapmis kullanicinin Id degerini okumaya calisir.
        /// </summary>
        /// <param name="userId">Basarili olursa kullanici Id degeri atanir.</param>
        /// <returns>Kullanici Id degeri okunabilirse true, aksi halde false doner.</returns>
        private bool TryGetCurrentUserId(out int userId)
        {
            userId = 0;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
         ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(userIdClaim, out userId);
        }

        /// <summary>
        /// Degerlendirme controller islemleri icin bilgi seviyesinde log kaydi olusturur.
        /// </summary>
        /// <param name="message">Log mesaji.</param>
        /// <param name="userId">Varsa islemi yapan kullanicinin Id degeri.</param>
        /// <param name="userEmail">Varsa islemi yapan kullanicinin e-posta adresi.</param>
        private async Task LogInfoAsync(string message, int? userId = null, string? userEmail = null)
        {
            await _logger.LogInfoAsync(
                message: message,
                userId: userId,
                userEmail: userEmail,
                controller: nameof(ReviewController),
                action: $"{Request.Method} {Request.Path}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
        }
    }
}
