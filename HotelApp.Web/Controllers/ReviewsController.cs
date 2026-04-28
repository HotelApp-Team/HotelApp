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

        [HttpPost]
        public async Task<IActionResult> Create(int hotelId, ReviewViewModel model)
        {
            var response = await _apiService.PostAsync<object>(
                $"hotels/{hotelId}/reviews", model);

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Yorum başarıyla eklandi.";
            }

            return RedirectToAction("HotelDetails", "Hotels", new { id = hotelId });
        }
    }
}
