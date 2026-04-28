using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers
{
    [Authorize]
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = GetCurrentUserId();

            var reservation = await _reservationService.GetUserReservationDetailsAsync(userId);

            return Ok(reservation);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound("Rezervasyon bulunamadı.");

            return Ok(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            var userId = GetCurrentUserId();
            var reservation = await _reservationService.CreateReservationAsync(userId, dto);

            return Ok(reservation);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetCurrentUserId();
            await _reservationService.CancelReservationAsync(id, userId);
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
