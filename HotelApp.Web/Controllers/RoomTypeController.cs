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
        public async Task<IActionResult> Create(RoomTypeAdminViewModel model)
        {
            await _apiService.PostAsync<object>("roomtypes", model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoomTypeAdminViewModel model)
        {
            await _apiService.PutAsync<object>($"roomtypes/{model.Id}", model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"roomtypes/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
