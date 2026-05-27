using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers
{
    /// <summary>
    /// Kullanici rezervasyonlarini listeleme, detay goruntuleme,
    /// fatura bilgisi alma, rezervasyon olusturma ve iptal etme islemlerini yonetir.
    /// </summary>
    [Authorize]
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILoggerService _logger;

        /// <summary>
        /// ReservationController sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="reservationService">Rezervasyon islemleri icin kullanilan servis.</param>
        /// <param name="logger">Loglama islemleri icin kullanilan servis.</param>
        public ReservationController(IReservationService reservationService, ILoggerService logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        /// <summary>
        /// Giris yapmis kullanicinin rezervasyon detaylarini getirir.
        /// </summary>
        /// <returns>Kullaniciya ait rezervasyon detay listesini doner.</returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var reservation = await _reservationService.GetUserReservationDetailsAsync(userId);

            return Ok(reservation);
        }

        /// <summary>
        /// Belirtilen Id degerine sahip rezervasyonu getirir.
        /// Kullanici yalnizca kendi rezervasyonunu, admin ise tum rezervasyonlari goruntuleyebilir.
        /// </summary>
        /// <param name="id">Getirilecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyon bilgilerini doner.</returns>
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

        /// <summary>
        /// Belirtilen rezervasyon icin fatura bilgisini getirir.
        /// Kullanici yalnizca kendi rezervasyon faturasini, admin ise tum faturalarin bilgisini goruntuleyebilir.
        /// </summary>
        /// <param name="id">Faturasi goruntulenecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyon fatura bilgisini doner.</returns>
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
                $"Rezervasyon faturası görüntülendi: ReservationId:{id}",
                userId: userId);

            return Ok(reservation);
        }

        /// <summary>
        /// Giris yapmis kullanici icin yeni rezervasyon olusturur.
        /// </summary>
        /// <param name="dto">Rezervasyon olusturma bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan rezervasyon bilgisini doner.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var reservation = await _reservationService.CreateReservationAsync(userId, dto);

            await LogInfoAsync(
                $"Yeni rezervasyon oluşturuldu: ReservationId:{reservation.Id}, HotelName:{reservation.HotelName}, HotelRoomId:{reservation.HotelRoomId}",
                userId: userId);

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }

        /// <summary>
        /// Belirtilen rezervasyonu iptal eder.
        /// Kullanici yalnizca kendi rezervasyonunu, admin ise tum rezervasyonlari iptal edebilir.
        /// </summary>
        /// <param name="id">Iptal edilecek rezervasyonun Id degeri.</param>
        /// <returns>Basarili olursa 204 NoContent doner.</returns>
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
        /// Rezervasyon controller islemleri icin bilgi seviyesinde log kaydi olusturur.
        /// </summary>
        /// <param name="message">Log mesajı.</param>
        /// <param name="userId">Varsa islemi yapan kullanicinin Id degeri.</param>
        /// <param name="userEmail">Varsa islemi yapan kullanicinin e-posta adresi.</param>
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
