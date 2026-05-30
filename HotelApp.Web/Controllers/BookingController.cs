using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    /// <summary>
    /// Web arayuzunde rezervasyon olusturma sayfasi ve rezervasyon kaydetme islemlerini yonetir.
    /// </summary>
    public class BookingController : Controller
    {
        private readonly ApiService _apiService;

        /// <summary>
        /// BookingController sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="apiService">API ile haberlesmek icin kullanilan servis.</param>
        public BookingController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Rezervasyon formundan gelen bilgileri API'ye gondererek yeni rezervasyon olusturur.
        /// Model ve tarih araligi validasyonu yapar; sonuc mesajini TempData uzerinden tasir.
        /// </summary>
        /// <param name="model">Rezervasyon olusturma bilgilerini iceren view model.</param>
        /// <returns>Basarili olursa rezervasyonlarim sayfasina, hata durumunda otel detay sayfasina yonlendirir.</returns>
        [ServiceFilter(typeof(AuthFilter))]
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
                TempData["Error"] = "Çıkış tarihi giriş tarihinden sonra olmalıdır";
                return RedirectToAction("Detail", "Hotels", new { id = model.HotelId });
            }

            var response = await _apiService.PostAsync<object>("reservations", model);

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction("Detail", "Hotels", new { id = model.HotelId });
            }

            TempData["Success"] = "Rezervasyon başarıyla oluşturuldu.";
            return RedirectToAction("My", "Reservations");
        }

        /// <summary>
        /// Rezervasyon olusturma sayfasini hazirlar.
        /// Secilen oda bilgilerini API'den alir ve tarih/misafir sayisi bilgileriyle view model olusturur.
        /// </summary>
        /// <param name="hotelId">Rezervasyon yapilacak otelin Id degeri.</param>
        /// <param name="hotelRoomId">Rezervasyon yapilacak otel odasinin Id degeri.</param>
        /// <param name="checkIn">Varsa secili giris tarihi.</param>
        /// <param name="checkOut">Varsa secili cikis tarihi.</param>
        /// <param name="guestCount">Varsa secili misafir sayisi.</param>
        /// <returns>Rezervasyon sayfasini veya hata durumunda otel detay sayfasina yonlendirme sonucunu doner.</returns>
        [ServiceFilter(typeof(AuthFilter))]
        [HttpGet]
        public async Task<IActionResult> Create(int hotelId, int hotelRoomId, DateTime? checkIn, DateTime? checkOut, int? guestCount)
        {
            var response = await _apiService.GetAsync<List<AvailableRoomViewModel>>($"hotels/{hotelId}/rooms");

            var room = response.Data?
                .Where(x => x.IsActive)
                .FirstOrDefault(x => x.Id == hotelRoomId || x.HotelRoomId == hotelRoomId);

            if (room == null)
            {
                TempData["Error"] = "Oda bilgisi bulunamadı.";
                return RedirectToAction("Detail", "Hotels", new { id = hotelId });
            }

            var model = new BookingPageViewModel
            {
                HotelId = hotelId,
                HotelRoomId = hotelRoomId,
                HotelName = room.HotelName,
                RoomTypeName = room.RoomTypeName,
                PricePerNight = room.PricePerNight,
                ExtraGuestPricePerNight = room.ExtraGuestPricePerNight,
                MaxGuests = room.MaxGuests,
                CheckInDate = DateOnly.FromDateTime(checkIn ?? DateTime.Today),
                CheckOutDate = DateOnly.FromDateTime(checkOut ?? DateTime.Today.AddDays(1)),
                GuestCount = guestCount ?? 1
            };

            return View(model);
        }
    }
}