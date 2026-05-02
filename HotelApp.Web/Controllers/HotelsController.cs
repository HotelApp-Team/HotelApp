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
            int? guestCount,
            string? sortBy,
            int page = 1)
        {
            // Zorunlu alanlar eksikse API'ye gitmeden once kullaniciya bilgi ver.
            // Boylece sayfa ilk acildiginda gereksiz 400 cagrisi yapmiyoruz.
            var hasRequiredFilters =
                !string.IsNullOrWhiteSpace(city) &&
                checkIn.HasValue &&
                checkOut.HasValue &&
                guestCount.HasValue;

            var model = new HotelSearchPageViewModel
            {
                City = city,
                StarRating = starRating,
                MinRating = minStarRating,
                CheckIn = checkIn,
                CheckOut = checkOut,
                GuestCount = guestCount,
                SortBy = sortBy,
                CurrentPage = page,
                PageSize = 9,
                TotalCount = 0,
                Hotels = new()
            };

            if (!hasRequiredFilters)
            {
                return View(model);
            }

            var queryParams = new Dictionary<string, string?>()
            {
                ["city"] = city,
                ["starRating"] = starRating?.ToString(),
                ["minStarRating"] = minStarRating?.ToString(),
                ["checkIn"] = checkIn?.ToString("yyyy-MM-dd"),
                ["checkOut"] = checkOut?.ToString("yyyy-MM-dd"),
                ["guestCount"] = guestCount?.ToString(),
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

            if (!response.Success)
            {
                // API'den gelen hatayi sayfada kullaniciya gosteriyoruz.
                TempData["SearchError"] = string.IsNullOrWhiteSpace(response.Message)
                    ? "Arama yapilirken bir sorun olustu, lutfen tekrar deneyin."
                    : response.Message;
            }
            else
            {
                model.TotalCount = response.Data?.TotalCount ?? 0;
                model.Hotels = response.Data?.Hotels ?? new();
            }

            return View(model);
        }
        public async Task<IActionResult> Detail(int id)
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
