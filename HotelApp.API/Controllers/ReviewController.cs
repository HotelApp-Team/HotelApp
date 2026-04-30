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

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [AllowAnonymous]
        [HttpGet("~/api/hotels/{hotelId}/reviews")]
        public async Task<IActionResult> GetHotelReviews(int hotelId)
        {
            var review = await _reviewService.GetHotelReviewAsync(hotelId);
            
            return Ok(review);
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

            return Created(string.Empty ,review);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            await _reviewService.DeleteReviewAsync(id, userId);

            return NoContent();
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            userId = 0;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
         ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(userIdClaim, out userId);
        }
    }
}
