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
        [HttpGet]
        public async Task<IActionResult> GetHotelReviews(int hotelId)
        {
            var review = await _reviewService.GetHotelReviewAsync(hotelId);
            
            return Ok(review);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = GetCurrentUserId();

            var review = await _reviewService.GetUserReviewsAsync(userId);

            return Ok(review);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            var userId = GetCurrentUserId();

            var review = await _reviewService.CreateReviewAsync(userId, dto);

            return Ok(review);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();

            await _reviewService.DeleteReviewAsync(id, userId);

            return NoContent();
        } 

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
         ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return int.Parse(userIdClaim!);
        }
    }
}
