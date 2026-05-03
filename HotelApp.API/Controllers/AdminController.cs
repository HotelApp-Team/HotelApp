using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers;

// Amaç: Admin paneli endpointlerini yönetir ve servis katmanından gelen sonucu API cevabına çevirir.
// SOLID: SRP ile sadece admin HTTP isteklerinden sorumludur; DIP ile IAdminService arayüzüne bağlı çalışır.

[Route("api/admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    // Admin işlemleri için sadece AdminService sözleşmesini alıyoruz.
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
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
        // Filtre ve sayfalama bilgilerini AdminService katmanına gönderiyoruz.
        var result = await _adminService.GetUsersForAdminAsync(search, isActive, page, pageSize);

        // Service'ten gelen güvenli DTO cevabını 200 OK ile döndürüyoruz.
        return Ok(result);
    }

    // E38 PUT api/admin/users/{id}/status: Kullanıcı hesabını aktif veya pasif yapar.
    [HttpPut("users/{id}/status")]
    public async Task<ActionResult<AdminUserDto>> UpdateUserStatus(int id, [FromBody] AdminUpdateUserStatusDto dto)
    {
        // Token içinden işlemi yapan admin kullanıcının Id bilgisini alıyoruz.
        if (!TryGetCurrentUserId(out var currentAdminId))
            return Unauthorized("Kullanıcı bilgisi token içinde bulunamadı.");

        try
        {
            var result = await _adminService.UpdateUserStatusAsync(id, dto, currentAdminId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // JWT token içindeki kullanıcı Id bilgisini güvenli şekilde okumaya çalışır.
    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId);
    }
}
