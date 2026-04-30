using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApiService _apiService;

        public ReviewsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(int hotelId, ReviewViewModel model)
        {
            model.HotelId = hotelId;

            var response = await _apiService.PostAsync<object>(
                "reviews", model);

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Yorum başarıyla eklendi.";
            }

            return RedirectToAction("Details", "Hotels", new { id = hotelId });
        }
    }
}
