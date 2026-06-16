using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers
{
    // Admin yorum yonetim ekrani: tum yorumlari listeler ve admin'in yorum silmesine izin verir.
    // AdminFilter sayesinde sadece "Admin" rolundeki kullanicilar bu sayfayi gorebilir.
    [Route("admin/reviews")]
    [ServiceFilter(typeof(AdminFilter))]
    public class AdminReviewsController : Controller
    {
        private readonly ApiService _apiService;

        public AdminReviewsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var endpoint = $"admin/reviews?page={page}&pageSize=10";

            var response = await _apiService.GetAsync<PagedResponseViewModel<AdminReviewViewModel>>(endpoint);

            // API hatasi olursa bos listeyi basmadan once admin'e nedeni gosterir.
            if (!response.Success)
            {
                TempData["Error"] = string.IsNullOrWhiteSpace(response.Message)
                    ? "Yorumlar getirilirken bir hata olustu."
                    : response.Message;

                return View(new PagedResponseViewModel<AdminReviewViewModel>
                {
                    Page = page,
                    PageSize = 10
                });
            }

            var model = response.Data ?? new PagedResponseViewModel<AdminReviewViewModel>
            {
                Page = page,
                PageSize = 10
            };

            return View(model);
        }

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // /api/reviews/{id} endpoint'i admin rolu ile gelen istegi servis tarafinda
            // isAdmin=true ile siler (kendi yorumu olmasa bile yetkilendirilir).
            var response = await _apiService.DeleteAsync($"reviews/{id}");

            TempData[response.Success ? "Success" : "Error"] =
                response.Success ? "Yorum silindi." : response.Message;

            return RedirectToAction(nameof(Index));
        }

        // Admin yorumunu onaylayarak otel detayi ve anasayfa vitrininde yayina alir.
        [HttpPost("approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var response = await _apiService.PutAsync($"admin/reviews/{id}/approve", new { });

            TempData[response.Success ? "Success" : "Error"] =
                response.Success ? "Yorum onaylandi ve yayina alindi." : response.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
