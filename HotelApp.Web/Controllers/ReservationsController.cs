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

        [HttpPost]
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            var response = await _apiService.PostAsync<object>("reservations", model);

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction("HotelDetails", "Hotels", new { id = model.HotelId });
            }

            TempData["Success"] = "Rezervasyon başarıyla oluşturuldu.";
            return RedirectToAction("HotelDetails", "Hotels", new { id = model.HotelId });
        }
    }
}
