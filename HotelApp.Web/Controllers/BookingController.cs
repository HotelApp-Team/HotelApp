using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApiService _apiService;

        public BookingController(ApiService apiService)
        {
            _apiService = apiService;
        }

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
    }
}
