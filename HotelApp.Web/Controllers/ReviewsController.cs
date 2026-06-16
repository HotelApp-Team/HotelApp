using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    /// <summary>
    /// Web arayuzunde kullanici yorumlarini listeleme, yorum olusturma
    /// ve yorum silme islemlerini yonetir.
    /// </summary>
    public class ReviewsController : Controller
    {
        private readonly ApiService _apiService;

        /// <summary>
        /// ReviewsController sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="apiService">API ile haberlesmek icin kullanilan servis.</param>
        public ReviewsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Giris yapmis kullanicinin kendi yorumlarini listeler.
        /// </summary>
        /// <returns>Kullanicinin yorumlari view sonucunu doner.</returns>
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

        /// <summary>
        /// Giris yapmis kullanici icin belirtilen otele yeni yorum ekler.
        /// Web projesinde session/JWT tabanli AuthFilter ile yetki kontrolu yapilir.
        /// </summary>
        /// <param name="hotelId">Yorum eklenecek otelin Id degeri.</param>
        /// <param name="model">Yorum bilgilerini iceren view model.</param>
        /// <returns>Otel detay sayfasina yonlendirir.</returns>
        [ServiceFilter(typeof(AuthFilter))]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(int hotelId, ReviewViewModel model)
        {
            model.HotelId = hotelId;

            var response = await _apiService.PostAsync<object>("reviews", model);

            TempData[response.Success ? "Success" : "Error"] =
                response.Success
                    ? "Yorumunuz alindi. Admin onayindan sonra yayina girecek."
                    : response.Message;

            return RedirectToAction("Detail", "Hotels", new { id = hotelId });
        }

        /// <summary>
        /// Belirtilen yorumu siler.
        /// Islem sonucunu TempData uzerinden kullaniciya gosterir.
        /// </summary>
        /// <param name="id">Silinecek yorumun Id degeri.</param>
        /// <returns>Kullanici yorumlari sayfasina yonlendirir.</returns>
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
