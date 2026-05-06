using HotelApp.Web.Filters;
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

        [ServiceFilter(typeof(AuthFilter))]
        public async Task<IActionResult> My()
        {
            var response = await _apiService.GetAsync<List<ReviewViewModel>>("reviews/my");

            if (!response.Success)
            {
                TempData["Error"] = response.Message;
                return View(new List<ReviewViewModel>());
            }

            return View(response.Data ?? new List<ReviewViewModel>());
        }

        [ServiceFilter(typeof(AuthFilter))]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(int hotelId, ReviewViewModel model)
        {
            model.HotelId = hotelId;

            var response = await _apiService.PostAsync<object>("reviews", model);

            TempData[response.Success ? "Success" : "Error"] =
                response.Success ? "Yorum basariyla eklendi." : response.Message;

            return RedirectToAction("Detail", "Hotels", new { id = hotelId });
        }

        [ServiceFilter(typeof(AuthFilter))]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"reviews/{id}");

            TempData[response.Success ? "Success" : "Error"] =
                response.Success ? "Yorum silindi." : response.Message;

            return RedirectToAction("My");
        }
    }
}
