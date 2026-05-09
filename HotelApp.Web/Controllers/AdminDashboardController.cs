using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

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

    public async Task<IActionResult> Index()
    {
        var response = await _apiService.GetAsync<AdminDashboardViewModel>("admin/dashboard");

        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return View(new AdminDashboardViewModel());
        }

        // API success dondurse bile body bos olabilir (204 No Content veya parse edilemeyen body).
        // Data null gelirse view @Model.X erisimlerinde NullReferenceException alir; bos VM ile koruruz.
        return View(response.Data ?? new AdminDashboardViewModel());
    }
}
