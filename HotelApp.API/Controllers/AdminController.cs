using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers;

// AdminController sınıfının amacı, admin paneli API endpointlerini yönetmektir.
// Dashboard verisi, kullanıcı listesi, kullanıcı durumu ve rol güncelleme isteklerini karşılar.
// SOLID: SRP kullanır; sadece admin HTTP isteklerinden sorumludur.
// SOLID: DIP kullanır; iş mantığı için somut sınıf yerine IAdminService arayüzüne bağlıdır.

[Route("api/admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILoggerService _logger;

    // AdminService is mantigini, LoggerService ise audit log yazimini saglar.
    public AdminController(IAdminService adminService, ILoggerService logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    // E36 - GET api/admin/dashboard : Admin dashboard istatistiklerini getirir.
    [HttpGet("dashboard")]
    public async Task<ActionResult> Dashboard()
    {
        var result = await _adminService.GetDashboardDataAsync();
        return Ok(result);
    }

    // E37 - GET api/admin/users : Admin paneli için kullanıcı listesini getirir.
    [HttpGet("users")]
    public async Task<ActionResult> GetUsersForAdminAsync(string? search = null, bool? isActive = null, int page = 1, int pageSize = 10)
    {
        // Filtre ve sayfalama bilgilerini AdminService katmanına gönderiyoruz.
        var result = await _adminService.GetUsersForAdminAsync(search, isActive, page, pageSize);

        // Service'ten gelen güvenli DTO cevabını 200 OK ile döndürüyoruz.
        return Ok(result);
    }

    // E38 - PUT api/admin/users/{id}/status : Kullanıcı hesabını aktif veya pasif yapar.
    [HttpPut("users/{id}/status")]
    public async Task<ActionResult<AdminUserDto>> UpdateUserStatus(int id, [FromBody] AdminUpdateUserStatusDto dto)
    {
        // Token içinden işlemi yapan admin kullanıcının Id bilgisini alıyoruz.
        if (!TryGetCurrentUserId(out var currentAdminId))
            return Unauthorized("Kullanıcı bilgisi token içinde bulunamadı.");

        var result = await _adminService.UpdateUserStatusAsync(id, dto, currentAdminId);

        await LogInfoAsync(
            $"Kullanıcı durumu değiştirildi: TargetUserId={id}, IsActive={dto.IsActive}, AdminId={currentAdminId}",
            userId: currentAdminId);

        return Ok(result);
    }

    // JWT token içindeki kullanıcı Id bilgisini güvenli şekilde okumaya çalışır.
    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId);
    }

    // E39 - PUT api/admin/users/{id}/roles : Kullanıcının rollerini günceller.
    [HttpPut("users/{id}/roles")]
    public async Task<ActionResult<AdminUserDto>> UpdateUserRoles(int id, [FromBody] AdminUpdateUserRolesDto dto)
    {
        if (!TryGetCurrentUserId(out var currentAdminId))
            return Unauthorized("Kullanıcı bilgisi token içinde bulunamadı.");

        var result = await _adminService.UpdateUserRolesAsync(id, dto, currentAdminId);

        await LogInfoAsync(
            $"Kullanıcı rolleri güncellendi: TargetUserId={id}, AdminId={currentAdminId}",
            userId: currentAdminId);

        return Ok(result);
    }

    // E44 - GET api/admin/logs : Sistem loglarını filtreli ve sayfalı şekilde getirir.
    [HttpGet("logs")]
    public async Task<ActionResult<PagedResponseDto<LogDto>>> GetLogs([FromQuery] LogFilterDto filter)
    {
        var result = await _adminService.GetLogsAsync(filter);

        // Admin paneline kim ne zaman baktigini audit izi icin tutuyoruz.
        if (TryGetCurrentUserId(out var currentAdminId))
        {
            await LogInfoAsync(
                $"Admin log listesi görüntülendi: Page={filter.PageNumber}, PageSize={filter.PageSize}",
                userId: currentAdminId);
        }

        return Ok(result);
    }

    // E45 - GET api/admin/logs/{id} : Tek log kaydını detaylı şekilde getirir.
    [HttpGet("logs/{id}")]
    public async Task<ActionResult<LogDto>> GetLogById(int id)
    {
        var log = await _adminService.GetLogByIdAsync(id);
        if (log == null)
            return NotFound();

        if (TryGetCurrentUserId(out var currentAdminId))
        {
            await LogInfoAsync(
                $"Admin log detayı görüntülendi: LogId={id}",
                userId: currentAdminId);
        }

        return Ok(log);
    }

    // Ortak bilgi logu atmak icin kullanilir.
    // Mesaj, kullanici bilgisi, controller/action ve IP bilgisini tek yerden logger servisine yollar.
    private async Task LogInfoAsync(string message, int? userId = null, string? userEmail = null)
    {
        await _logger.LogInfoAsync(
            message: message,
            userId: userId,
            userEmail: userEmail,
            controller: nameof(AdminController),
            action: $"{Request.Method} {Request.Path}",
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
    }
}
