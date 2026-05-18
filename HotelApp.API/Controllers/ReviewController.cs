using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILoggerService _logger;

        public ReviewController(IReviewService reviewService, ILoggerService logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        // E53 - GET api/reviews/featured : Anasayfada gosterilecek one cikan yorumlari getirir.
        [AllowAnonymous]
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedReviews([FromQuery] int count = 3)
        {
            var reviews = await _reviewService.GetFeaturedReviewsAsync(count);

            return Ok(reviews);
        }

        [AllowAnonymous]
        [HttpGet("~/api/hotels/{hotelId}/reviews")]
        public async Task<IActionResult> GetHotelReviews(int hotelId)
        {
            var review = await _reviewService.GetHotelReviewAsync(hotelId);
            
            return Ok(review);
        }

        // Admin yorum yonetim sayfasi icin sayfali tum yorum listesi.
        // Route prefix'i tilde ile bypass edip /api/admin/reviews adresine bagliyoruz.
        [Authorize(Roles = "Admin")]
        [HttpGet("~/api/admin/reviews")]
        public async Task<IActionResult> GetAllForAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _reviewService.GetAllReviewsAsync(page, pageSize);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyReviews()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var review = await _reviewService.GetUserReviewsAsync(userId);

            return Ok(review);
        }

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

        private bool TryGetCurrentUserId(out int userId)
        {
            userId = 0;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
         ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(userIdClaim, out userId);
        }

        // Ortak bilgi logu atmak icin kullanilir.
        // Mesaj, kullanici bilgisi, controller/action ve IP bilgisini tek yerden logger servisine yollar.
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
