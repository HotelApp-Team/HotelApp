using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    /// <summary>
    /// Admin panelinde rezervasyon listeleme, filtreleme ve durum guncelleme islemlerini yonetir.
    /// </summary>
    [Route("admin/reservations")]
    [ServiceFilter(typeof(AdminFilter))]
    public class AdminReservationsController : Controller
    {
        private readonly ApiService _apiService;

        /// <summary>
        /// AdminReservationsController sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="apiService">API ile haberlesmek icin kullanilan servis.</param>
        public AdminReservationsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Admin rezervasyon yonetim sayfasini filtre ve sayfalama bilgilerine gore getirir.
        /// </summary>
        /// <param name="status">Opsiyonel rezervasyon durumu filtresi.</param>
        /// <param name="hotelId">Opsiyonel otel Id filtresi.</param>
        /// <param name="from">Opsiyonel baslangic tarihi filtresi.</param>
        /// <param name="to">Opsiyonel bitis tarihi filtresi.</param>
        /// <param name="page">Getirilecek sayfa numarasi.</param>
        /// <returns>Admin rezervasyon yonetim view sonucunu doner.</returns>
        [HttpGet("")]
        public async Task<IActionResult> Index(string? status, int? hotelId, DateTime? from, DateTime? to, int page = 1)
        {
            var endpoint = $"admin/reservations?page={page}&pageSize=10";

            if (!string.IsNullOrWhiteSpace(status))
                endpoint += $"&status={Uri.EscapeDataString(status)}";

            if (hotelId.HasValue)
                endpoint += $"&hotelId={hotelId.Value}";

            if (from.HasValue)
                endpoint += $"&from={from.Value:yyyy-MM-dd}";

            if (to.HasValue)
                endpoint += $"&to={to.Value:yyyy-MM-dd}";

            var response = await _apiService.GetAsync<PagedResponseViewModel<ReservationAdminViewModel>>(endpoint);

            ViewBag.Status = status;
            ViewBag.HotelId = hotelId;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            var model = response.Data ?? new PagedResponseViewModel<ReservationAdminViewModel>();

            return View(model);
        }

        /// <summary>
        /// Belirtilen rezervasyonun durumunu admin API uzerinden gunceller.
        /// </summary>
        /// <param name="id">Durumu guncellenecek rezervasyonun Id degeri.</param>
        /// <param name="status">Yeni rezervasyon durumu.</param>
        /// <returns>Rezervasyon yonetim sayfasina yonlendirir.</returns>
        [HttpPost("update-status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _apiService.PutAsync<object>($"admin/reservations/{id}/status", new { status });
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Belirtilen rezervasyonu onaylar veya onayini kaldirir.
        /// </summary>
        /// <param name="id">Onay durumu guncellenecek rezervasyonun Id degeri.</param>
        /// <param name="confirm">true ise onaylar, false ise onayi kaldirir.</param>
        /// <returns>Rezervasyon yonetim sayfasina yonlendirir.</returns>
        [HttpPost("confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id, bool confirm, string? status, int? hotelId, DateTime? from, DateTime? to, int page = 1)
        {
            await _apiService.PutAsync<object>($"admin/reservations/{id}/confirm", new { confirm });

            // Onay sonrasi kullaniciyi geldigi sayfa ve filtrelerle ayni listeye geri dondur
            // (parametresiz Index'e donmek 1. sayfaya/filtresiz hale atiyordu).
            return RedirectToAction(nameof(Index), new { status, hotelId, from, to, page });
        }
    }
}