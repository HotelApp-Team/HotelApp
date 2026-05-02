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
    private readonly IUserService _userService;

    // Admin ve kullanıcı servislerini controller'a alır.
    public AdminController(IAdminService adminService, IUserService userService)
    {
        _adminService = adminService;
        _userService = userService;
    }

    // E36 GET api/admin/dashboard: Admin dashboard istatistiklerini getirir ve 200 OK olarak döndürür.
    [HttpGet("dashboard")]
    public async Task<ActionResult> Dashboard()
    {
        var result = await _adminService.GetDashboardDataAsync();
        return Ok(result);
    }

    // E37 GET api/admin/users: Admin paneli için kullanıcı listesini getirir.
    [HttpGet("users")]
    public async Task<ActionResult> GetUsersForAdminAsync(string? search = null, bool? isActive = null, int page = 1, int pageSize = 10)
    {
        // Filtre ve sayfalama bilgilerini service katmanına gönderiyoruz.
        var result = await _userService.GetUsersForAdminAsync(search, isActive, page, pageSize);

        // Service'ten gelen güvenli DTO cevabını 200 OK ile döndürüyoruz.
        return Ok(result);
    }
}
