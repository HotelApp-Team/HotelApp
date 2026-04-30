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
        [HttpGet]
        public async Task<IActionResult> Search(
            string? city,
            byte? starRating,
            decimal? minStarRating,
            DateTime? checkIn,
            DateTime? checkOut,
            int? guests,
            string? sortBy,
            int page = 1)
        {
            var queryParams = new Dictionary<string, string?>()
            {
                ["city"] = city,
                ["starRating"] = starRating?.ToString(),
                ["minStarRating"] = minStarRating?.ToString(),
                ["checkIn"] = checkIn?.ToString("yyyy-MM-dd"),
                ["checkOut"] = checkOut?.ToString("yyyy-MM-dd"),
                ["guestCount"] = guests?.ToString(),
                ["sortBy"] = sortBy,
                ["page"] = page.ToString(),
                ["pageSize"] = "9"
            };

            var query = "hotels/search?" +
                        string.Join("&",
                            queryParams
                                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));

            var response = await _apiService.GetAsync<HotelSearchResponseViewModel>(query);

            var model = new HotelSearchPageViewModel
            {
                City = city,
                StarRating = starRating,
                MinRating = minStarRating,
                CheckIn = checkIn,
                CheckOut = checkOut,
                GuestCount = guests,
                SortBy = sortBy,
                CurrentPage = page,
                PageSize = 9,
                TotalCount = response.Success ? response.Data?.TotalCount ?? 0 : 0,
                Hotels = response.Success ? response.Data?.Hotels ?? new() : new()
            };

            return View(model);
        }
        public async Task<IActionResult> Details(int id)
        {
            var hotelResponse = await _apiService.GetAsync<HotelDetailPageVm>($"Hotels/{id}");
            var imagesResponse = await _apiService.GetAsync<List<HotelImageViewModel>>($"hotels/{id}/images");
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

            model.Id = id;
            model.HotelId = id;
            model.HotelName = string.IsNullOrWhiteSpace(model.HotelName)
                ? model.Name
                : model.HotelName;

            model.Images = imagesResponse.Success
                ? imagesResponse.Data ?? new List<HotelImageViewModel>()
                : new List<HotelImageViewModel>();

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
