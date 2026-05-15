using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelApp.Web.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    public class RoomTypeController : Controller
    {
        private readonly ApiService _apiService;

        public RoomTypeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<List<RoomTypeAdminViewModel>>("roomtypes");
            return View("~/Views/Admin/RoomTypes.cshtml", response.Data ?? new List<RoomTypeAdminViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomTypeAdminViewModel model)
        {
            var response = await _apiService.PostAsync<object>("roomtypes", model);

            if (!response.Success)
            {
                TempData["Error"] = response.Message ?? "Oda tipi eklenemedi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Oda tipi eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RoomTypeAdminViewModel model)
        {
            var response = await _apiService.PutAsync<object>($"roomtypes/{model.Id}", model);

            if (!response.Success)
            {
                TempData["Error"]=response.Message ?? "Oda tipi güncellenemedi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Oda tipi güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"roomtypes/{id}");

            if(!response.Success)
            {
                TempData["Error"] = response.Message ?? "Oda tipi silinemedi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Oda tipi silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
