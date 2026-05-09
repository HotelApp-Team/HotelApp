using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// AdminDashboardController sınıfının amacı, admin dashboard sayfasını API verisiyle göstermektir.
// E36 dashboard istatistiklerini web tarafında çağırır ve boş veri durumunda sayfanın bozulmasını engeller.
// SOLID: SRP kullanır; sadece admin dashboard sayfa akışından sorumludur.

[Route("admin/dashboard")]
[ServiceFilter(typeof(AdminFilter))]
[ApiExplorerSettings(IgnoreApi = true)]
public class AdminDashboardController : Controller
{
    private readonly ApiService _apiService;

    public AdminDashboardController(ApiService apiService)
    {
        _apiService = apiService;
    }

    // E36 için admin dashboard istatistiklerini API'den alıp view'a gönderir.
    public async Task<IActionResult> Index()
    {
        var response = await _apiService.GetAsync<AdminDashboardViewModel>("admin/dashboard");

        // API çağrısı başarısızsa sayfayı boş modelle açar ve kullanıcıya hata mesajı gösterir.
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return View(new AdminDashboardViewModel());
        }

        // API success döndürse bile body boş olabilir (204 No Content veya parse edilemeyen body).
        // Data null gelirse view @Model.X erişimlerinde NullReferenceException alır; boş VM ile koruruz.
        return View(response.Data ?? new AdminDashboardViewModel());
    }
}
