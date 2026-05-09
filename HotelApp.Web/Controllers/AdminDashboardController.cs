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

        return View(response.Data);
    }
}
