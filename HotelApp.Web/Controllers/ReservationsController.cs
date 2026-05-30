using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    /// <summary>
    /// Web arayuzunde giris yapmis kullanicinin rezervasyonlarini listeleme,
    /// detay goruntuleme, fatura goruntuleme ve iptal etme islemlerini yonetir.
    /// </summary>
    [ServiceFilter(typeof(AuthFilter))]
    public class ReservationsController : Controller
    {
        private readonly ApiService _apiService;

        /// <summary>
        /// ReservationsController sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="apiService">API ile haberlesmek icin kullanilan servis.</param>
        public ReservationsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Giris yapmis kullanicinin rezervasyonlarini filtre ve sayfalama bilgilerine gore getirir.
        /// </summary>
        /// <param name="status">Opsiyonel rezervasyon durumu filtresi.</param>
        /// <param name="sortBy">Opsiyonel siralama kriteri.</param>
        /// <param name="page">Getirilecek sayfa numarasi.</param>
        /// <param name="pageSize">Sayfa basina kayit sayisi.</param>
        /// <returns>Kullanici rezervasyonlari view sonucunu doner.</returns>
        public async Task<IActionResult> My(string? status, string? sortBy, int page = 1, int pageSize = 10)
        {
            var endpoint = $"reservations/my?status={status}&sortBy={sortBy}&page={page}&pageSize={pageSize}";
            var response = await _apiService.GetAsync<List<ReservationDetailViewModel>>(endpoint);

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
                return View(new List<ReservationDetailViewModel>());
            }

            return View(response.Data ?? new List<ReservationDetailViewModel>());
        }

        /// <summary>
        /// Belirtilen rezervasyonu iptal eder.
        /// Islem sonucunu TempData uzerinden kullaniciya gosterir.
        /// </summary>
        /// <param name="id">Iptal edilecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyonlarim sayfasina yonlendirir.</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = await _apiService.PutAsync($"reservations/{id}/cancel", new { });

            TempData[response.Success ? "Success" : "Error"] =
                response.Success ? "Rezervasyon iptal edildi." : response.Message;

            return RedirectToAction("My");
        }

        /// <summary>
        /// Belirtilen rezervasyonun detay bilgisini getirir.
        /// </summary>
        /// <param name="id">Detayi goruntulenecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyon detay view sonucunu doner.</returns>
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _apiService.GetAsync<ReservationDetailViewModel>($"reservations/{id}");

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Rezervasyon detayi alinamadi.";
                return RedirectToAction("My");
            }

            // API DTO -> ViewModel JSON mapping'inde rezervasyon id'si bos kalabiliyor
            // (DTO'da farkli isimde alan). Endpoint URL'inden gelen id zaten dogru,
            // gosterimde kullanilabilsin diye direkt ata.
            response.Data.ReservationId = id;

            return View(response.Data);
        }

        /// <summary>
        /// Belirtilen rezervasyonun fatura bilgisini getirir.
        /// </summary>
        /// <param name="id">Faturasi goruntulenecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyon fatura view sonucunu doner.</returns>
        public async Task<IActionResult> Invoice(int id)
        {
            var response = await _apiService.GetAsync<ReservationDetailViewModel>($"reservations/{id}/invoice");

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Fatura bilgisi alinamadi.";
                return RedirectToAction("My");
            }

            // Detail ile ayni neden: URL'deki id'yi gosterim icin ViewModel'a aktar
            // (fatura ekraninda "Rezervasyon No: #..." dogru cikinsin).
            response.Data.ReservationId = id;

            return View(response.Data);
        }
    }
}
