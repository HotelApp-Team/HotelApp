using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    public class ReservationMgmtController : Controller
    {
        private readonly ApiService _apiService;

        public ReservationMgmtController(ApiService apiService)
        {
            _apiService = apiService;
        }

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

            return View("~/Views/Admin/Reservations.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _apiService.PutAsync<object>($"admin/reservations/{id}/status", new { status });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id, bool confirm)
        {
            await _apiService.PutAsync<object>($"admin/reservations/{id}/confirm", new { confirm });
            return RedirectToAction(nameof(Index));
        }
    }
}
