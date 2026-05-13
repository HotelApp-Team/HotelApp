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
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Rezervasyon bilgileri eksik veya hatalı.";
                return RedirectToAction("Detail", "Hotels", new { id = model.HotelId });
            }

            if (model.CheckOutDate <= model.CheckInDate)
            {
                TempData["Error"] = "Çıkış tarihi giriş tarihinden sonra olmalıdır.";
                return RedirectToAction("Detail", "Hotels", new { id = model.HotelId });
            }

            var response = await _apiService.PostAsync<object>("reservations", model);

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction("Detail", "Hotels", new { id = model.HotelId });
            }

            TempData["Success"] = "Rezervasyon başarıyla oluşturuldu.";
            return RedirectToAction("My");
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

            return View(response.Data);
        }
    }
}
