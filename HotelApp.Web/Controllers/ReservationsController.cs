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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            var response = await _apiService.PostAsync<object>("reservations", model);

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction("Detail", "Hotels", new { id = model.HotelId });
            }

            TempData["Success"] = "Rezervasyon başarıyla oluşturuldu.";
            return RedirectToAction("Detail", "Hotels", new { id = model.HotelId });
        }
    }
}
