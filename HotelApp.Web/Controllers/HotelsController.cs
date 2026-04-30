using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    public class HotelsController : Controller
    {
        private readonly ApiService _apiService;

        public HotelsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var hotelResponse = await _apiService.GetAsync<HotelDetailPageVm>($"Hotels/{id}");
            var roomsResponse = await _apiService.GetAsync<List<AvailableRoomViewModel>>($"hotels/{id}/rooms");
            var reviewsResponse = await _apiService.GetAsync<List<ReviewViewModel>>($"hotels/{id}/reviews");

            if (!hotelResponse.Success || hotelResponse.Data == null)
            {
                TempData["Error"] = "Otel bilgileri alınmadı.";
                return RedirectToAction("Index", "Home");
            }

            if (!roomsResponse.Success)
            {
                TempData["Error"] = roomsResponse.Message ?? "Otel odaları alınmadı.";
                return RedirectToAction("Index", "Home");
            }

            if (!reviewsResponse.Success)
            {
                TempData["Error"] = reviewsResponse.Message ?? "Otel yorumları alınmadı.";
            }

            var model = hotelResponse.Data;

            model.HotelId = id;
            model.HotelName = string.IsNullOrWhiteSpace(model.HotelName)
                ? model.Name
                : model.HotelName;

            model.Rooms = roomsResponse.Data ?? new List<AvailableRoomViewModel>();
            model.Reviews = reviewsResponse.Data ?? new List<ReviewViewModel>();
            model.BookingForm = new CreateReservationViewModel
            {
                HotelId = id
            };

            model.IsAuthenticated = HttpContext.Session.GetString("JWToken") != null;

            return View("Detail", model);
        }
    }
}
