using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    // Kullanıcı tarafındaki otel arama ve otel detay sayfalarını yöneten controller'dır.
    // Web katmanı API ile doğrudan değil, ApiService üzerinden haberleşir.
    public class HotelsController : Controller
    {
        private readonly ApiService _apiService;

        // API isteklerini yapabilmek için ApiService constructor ile alınır.
        public HotelsController(ApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        // Otel arama sayfasını yönetir.
        // Kullanıcının şehir, tarih, misafir sayısı ve filtre bilgilerine göre API'den otel araması yapar.
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

            // Sayfaya geri gönderilecek arama modelini oluşturur.
            // Kullanıcının girdiği filtreler modelde tutulur ki form değerleri kaybolmasın.
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

            // Boş olmayan filtreleri query string formatına çevirmek için sözlük kullanılır.
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

            // Query string oluşturulurken boş değerler gönderilmez.
            // Uri.EscapeDataString özel karakterlerin URL içinde güvenli taşınmasını sağlar.
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

        // Navbar üzerinden yapılan otel veya şehir aramasını karşılar.
        // API'den gelen sonuçları CatalogSearch view'ına gönderir.
        [HttpGet]
        public async Task<IActionResult> CatalogSearch(string? searchTerm, int page = 1)
        {
            // Her sayfada gösterilecek otel sayısıdır.
            const int pageSize = 100;

            // Kullanıcının arama ve sayfalama bilgilerini modele aktarır.
            var model = new CatalogSearchPageViewModel
            {
                SearchTerm = searchTerm?.Trim() ?? string.Empty,
                CurrentPage = page < 1 ? 1 : page,
                PageSize = pageSize
            };

            // Arama alanı boşsa API isteği göndermeden sayfayı açar.
            if (string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                return View(model);
            }

            // Özel Türkçe karakterlerin query string içinde güvenli taşınmasını sağlar.
            var encodedSearchTerm = Uri.EscapeDataString(model.SearchTerm);

            // Katalog arama API adresini oluşturur.
            var endpoint =
                "hotels/catalog-search" +
                $"?searchTerm={encodedSearchTerm}" +
                $"&page={model.CurrentPage}" +
                $"&pageSize={pageSize}";

            // API üzerinden sayfalı otel listesini getirir.
            var response = await _apiService.GetAsync<PagedHotelListResponse>(
                endpoint);

            // API başarısız olursa kullanıcıya hata mesajı gönderir.
            if (!response.Success || response.Data == null)
            {
                TempData["SearchError"] =
                    string.IsNullOrWhiteSpace(response.Message)
                    ? "Arama yapılırken bir sorun oluştu."
                    : response.Message;

                return View(model);
            }

            // API sonucunu sayfada kullanılacak modele aktarır.
            model.TotalCount = response.Data.TotalCount;
            model.Hotels = response.Data.Data;

            return View(model);
        }

        // Detail action: tarih ve misafir sayisi model binding ile sorgu parametrelerinden otomatik alinir.
        // Sevde'nin Request.Query[] ile manuel parse etmek istedigi davranis burada parametre binding ile saglaniyor.
        // Otel detay sayfası için otel bilgisi, görseller, odalar ve yorumlar API'den alınır.
        public async Task<IActionResult> Detail(int id, DateTime? checkIn, DateTime? checkOut, int? guestCount)
        {
            var availabilitySearchRequested =
                checkIn.HasValue ||
                checkOut.HasValue ||
                guestCount.HasValue;

            var validAvailabilitySearch =
                checkIn.HasValue &&
                checkOut.HasValue &&
                guestCount is >= 1 and <= 20 &&
                checkIn.Value.Date >= DateTime.Today &&
                checkOut.Value.Date > checkIn.Value.Date;

            var hotelResponse = await _apiService.GetAsync<HotelDetailPageVm>($"Hotels/{id}");
            var imagesResponse = await _apiService.GetAsync<List<HotelImageViewModel>>($"hotels/{id}/images");
            var roomsResponse = await _apiService.GetAsync<List<AvailableRoomViewModel>>($"hotels/{id}/rooms");
            var reviewsResponse = await _apiService.GetAsync<List<ReviewViewModel>>($"hotels/{id}/reviews");

            if (!hotelResponse.Success || hotelResponse.Data == null)
            {
                TempData["Error"] = "Otel bilgileri alınmadı.";
                return RedirectToAction("Index", "Home");
            }

            if (!roomsResponse.Success && !validAvailabilitySearch)
            {
                TempData["Error"] = roomsResponse.Message ?? "Otel odaları alınmadı.";
                return RedirectToAction("Index", "Home");
            }

            if (!reviewsResponse.Success)
            {
                TempData["Error"] = reviewsResponse.Message ?? "Otel yorumları alınmadı.";
            }

            var model = hotelResponse.Data;

            // API'den gelen modelin temel Id ve otel adı alanları garantiye alınır.
            model.Id = id;
            model.HotelId = id;
            model.HotelName = string.IsNullOrWhiteSpace(model.HotelName)
                ? model.Name
                : model.HotelName;

            // Görseller başarılı geldiyse modele eklenir, hata durumunda boş liste kullanılır.
            model.Images = imagesResponse.Success
                ? imagesResponse.Data ?? new List<HotelImageViewModel>()
                : new List<HotelImageViewModel>();

            // Sadece aktif odalar detay sayfasında gösterilir.
            if (availabilitySearchRequested && !validAvailabilitySearch)
            {
                TempData["SearchError"] = "Geçerli bir giriş tarihi, çıkış tarihi ve misafir sayısı giriniz.";
                model.Rooms = new List<AvailableRoomViewModel>();
            }
            else if (validAvailabilitySearch)
            {
                var availableRoomsResponse = await _apiService.PostAsync<List<AvailableRoomViewModel>>(
                    $"hotels/{id}/rooms/search",
                    new
                    {
                        City = model.City,
                        CheckIn = checkIn!.Value.Date,
                        CheckOut = checkOut!.Value.Date,
                        GuestCount = guestCount!.Value
                    });

                if (!availableRoomsResponse.Success)
                {
                    TempData["SearchError"] = availableRoomsResponse.Message ?? "Uygun odalar aranırken bir sorun oluştu.";
                    model.Rooms = new List<AvailableRoomViewModel>();
                }
                else
                {
                    model.Rooms = availableRoomsResponse.Data ?? new List<AvailableRoomViewModel>();
                }
            }
            else
            {
                model.Rooms = roomsResponse.Success
                    ? (roomsResponse.Data ?? new List<AvailableRoomViewModel>())
                        .Where(x => x.IsActive)
                        .ToList()
                    : new List<AvailableRoomViewModel>();
            }

            // Yorumlar alınamazsa sayfa tamamen kırılmasın diye boş liste atanır
            model.Reviews = reviewsResponse.Success
                ? reviewsResponse.Data ?? new List<ReviewViewModel>()
                : new List<ReviewViewModel>();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var defaultCheckIn = checkIn.HasValue
                ? DateOnly.FromDateTime(checkIn.Value)
                : today;

            var defaultCheckOut = checkOut.HasValue
                ? DateOnly.FromDateTime(checkOut.Value)
                : defaultCheckIn.AddDays(1);

            if (!availabilitySearchRequested && defaultCheckOut <= defaultCheckIn)
            {
                defaultCheckOut = defaultCheckIn.AddDays(1);
            }

            // Rezervasyon formu aramadan gelen tarih ve misafir bilgileriyle doldurulur.
            // Bilgi gelmezse varsayılan olarak bugünden yarına ve 1 misafir seçilir.
            model.BookingForm = new CreateReservationViewModel
            {
                HotelId = id,
                CheckInDate = defaultCheckIn,
                CheckOutDate = defaultCheckOut,
                GuestCount = guestCount.GetValueOrDefault(1)
            };

            // Kullanıcının giriş yapıp yapmadığı session içindeki JWT token ile kontrol edilir.
            model.IsAuthenticated = HttpContext.Session.GetString("JWToken") != null;

            // Arama ekranından gelen değerler detay sayfasında korunur.
            model.SearchCheckIn = checkIn;
            model.SearchCheckOut = checkOut;
            model.SearchGuestCount = guestCount;


            return View("Detail", model);
        }
    }
}
