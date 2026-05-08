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
        private readonly ILoggerService _logger;

        public ReservationController(IReservationService reservationService, ILoggerService logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var reservation = await _reservationService.GetUserReservationDetailsAsync(userId);

            return Ok(reservation);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound("Rezervasyon bulunamadı.");

            if (reservation.UserId != userId && !isAdmin)
                return Forbid();

            return Ok(reservation);
        }

        [HttpGet("{id}/invoice")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound("Rezervasyon bulunamadı.");

            if (reservation.UserId != userId && !isAdmin)
                return Forbid();

            await LogInfoAsync(
                $"Rezervasyon faturası görüntülendi: ReservationId={id}",
                userId: userId);

            return Ok(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var reservation = await _reservationService.CreateReservationAsync(userId, dto);

            await LogInfoAsync(
                $"Yeni rezervasyon oluşturuldu: ReservationId={reservation.Id}, HotelName={reservation.HotelName}, HotelRoomId={reservation.HotelRoomId}",
                userId: userId);

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            await _reservationService.CancelReservationAsync(id, userId, isAdmin);

            await LogInfoAsync(
                $"Rezervasyon iptal edildi: ReservationId={id}, isAdmin={isAdmin}",
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

        private async Task LogInfoAsync(string message, int? userId = null, string? userEmail = null)
        {
            await _logger.LogInfoAsync(
                message: message,
                userId: userId,
                userEmail: userEmail,
                controller: nameof(ReservationController),
                action: $"{Request.Method} {Request.Path}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
        }
    }
}
