using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelApp.Web.Controllers
{
    /// <summary>
    /// Admin panelinde oda tipi listeleme, ekleme, guncelleme ve silme islemlerini yonetir.
    /// </summary>
    [ServiceFilter(typeof(AdminFilter))]
    public class RoomTypeController : Controller
    {
        private readonly ApiService _apiService;

        /// <summary>
        /// RoomTypeController sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="apiService">API ile haberlesmek icin kullanilan servis.</param>
        public RoomTypeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Admin panelinde tum oda tiplerini listeler.
        /// </summary>
        /// <returns>Oda tipleri yonetim view sonucunu doner.</returns>
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<List<RoomTypeAdminViewModel>>("roomtypes");
            return View("~/Views/Admin/RoomTypes.cshtml", response.Data ?? new List<RoomTypeAdminViewModel>());
        }

        /// <summary>
        /// Yeni oda tipi olusturur.
        /// Islem sonucunu TempData uzerinden kullaniciya gosterir.
        /// </summary>
        /// <param name="model">Olusturulacak oda tipi bilgilerini iceren view model.</param>
        /// <returns>Oda tipi yonetim sayfasina yonlendirir.</returns>
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

        /// <summary>
        /// Mevcut oda tipi bilgilerini gunceller.
        /// Islem sonucunu TempData uzerinden kullaniciya gosterir.
        /// </summary>
        /// <param name="model">Guncellenecek oda tipi bilgilerini iceren view model.</param>
        /// <returns>Oda tipi yonetim sayfasina yonlendirir.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RoomTypeAdminViewModel model)
        {
            var response = await _apiService.PutAsync<object>($"roomtypes/{model.Id}", model);

            if (!response.Success)
            {
                TempData["Error"] = response.Message ?? "Oda tipi güncellenemedi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Oda tipi güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Belirtilen oda tipini siler.
        /// Islem sonucunu TempData uzerinden kullaniciya gosterir.
        /// </summary>
        /// <param name="id">Silinecek oda tipinin Id degeri.</param>
        /// <returns>Oda tipi yonetim sayfasina yonlendirir.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"roomtypes/{id}");

            if (!response.Success)
            {
                TempData["Error"] = response.Message ?? "Oda tipi silinemedi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Oda tipi silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}