using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

// Amaç: Admin paneli endpointlerini yönetir ve servis katmanından gelen sonucu API cevabına çevirir.
// SOLID: SRP ile sadece admin HTTP isteklerinden sorumludur; DIP ile IAdminService arayüzüne bağlı çalışır.

[Route("api/admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    // Admin servis bağımlılığını alır.
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // E36: Admin dashboard istatistiklerini getirir ve 200 OK olarak döndürür.
    [HttpGet("dashboard")]
    public async Task<ActionResult> Dashboard()
    {
        var result = await _adminService.GetDashboardDataAsync();
        return Ok(result);
    }
}
