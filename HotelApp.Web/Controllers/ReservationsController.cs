using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApiService _apiService;

        public ReservationsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [ServiceFilter(typeof(AuthFilter))]
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

        [ServiceFilter(typeof(AuthFilter))]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = await _apiService.PutAsync($"reservations/{id}/cancel", new { });

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Rezervasyon iptal edildi.";
            }

            return RedirectToAction("My");
        }

        [ServiceFilter(typeof(AuthFilter))]
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _apiService.GetAsync<ReservationDetailViewModel>($"reservations/{id}");

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Rezervasyon detayı alınamadı.";
                return RedirectToAction("My");
            }

            return View(response.Data);
        }

        [ServiceFilter(typeof(AuthFilter))]
        public async Task<IActionResult> Invoice(int id)
        {
            var response = await _apiService.GetAsync<ReservationDetailViewModel>($"reservations/{id}/invoice");

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = response.Message ?? "Fatura bilgisi alınamadı.";
                return RedirectToAction("My");
            }

            return View(response.Data);
        }
    }
}
