using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    // Rezervasyon islemleri sadece giris yapmis kullaniciya ozeldir.
    // AuthFilter, session'daki JWT token gecerli degilse Login sayfasina yonlendirir.
    [ServiceFilter(typeof(AuthFilter))]
    public class ReservationsController : Controller
    {
        private readonly ApiService _apiService;

        public ReservationsController(ApiService apiService)
        {
            _apiService = apiService;
        }

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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = await _apiService.PutAsync($"reservations/{id}/cancel", new { });

            TempData[response.Success ? "Success" : "Error"] =
                response.Success ? "Rezervasyon iptal edildi." : response.Message;

            return RedirectToAction("My");
        }

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
