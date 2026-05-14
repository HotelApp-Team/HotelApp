using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using HotelApp.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers;

[Route("api/admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILoggerService _logger;

    public AdminController(IAdminService adminService, ILoggerService logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> Dashboard()
    {
        var result = await _adminService.GetDashboardDataAsync();
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<ActionResult<AdminUsersResponseDto>> GetUsersForAdminAsync(string? search = null, bool? isActive = null, int page = 1, int pageSize = 10)
    {
        var result = await _adminService.GetUsersForAdminAsync(search, isActive, page, pageSize);
        return Ok(result);
    }

    [HttpPut("users/{id}/status")]
    public async Task<ActionResult<AdminUserDto>> UpdateUserStatus(int id, [FromBody] AdminUpdateUserStatusDto dto)
    {
        if (!TryGetCurrentUserId(out var currentAdminId))
            return Unauthorized("Kullanici bilgisi token icinde bulunamadi.");

        var result = await _adminService.UpdateUserStatusAsync(id, dto, currentAdminId);

        await LogInfoAsync(
            $"Kullanici durumu degistirildi: TargetUserId={id}, IsActive={dto.IsActive}, AdminId={currentAdminId}",
            userId: currentAdminId);

        return Ok(result);
    }

    [HttpPut("users/{id}/roles")]
    public async Task<ActionResult<AdminUserDto>> UpdateUserRoles(int id, [FromBody] AdminUpdateUserRolesDto dto)
    {
        if (!TryGetCurrentUserId(out var currentAdminId))
            return Unauthorized("Kullanici bilgisi token icinde bulunamadi.");

        var result = await _adminService.UpdateUserRolesAsync(id, dto, currentAdminId);

        await LogInfoAsync(
            $"Kullanici rolleri guncellendi: TargetUserId={id}, AdminId={currentAdminId}",
            userId: currentAdminId);

        return Ok(result);
    }

    [HttpGet("reservations")]
    public async Task<IActionResult> GetReservations(
        string? status = null,
        int? hotelId = null,
        DateTime? from = null,
        DateTime? to = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _adminService.GetReservationsAsync(status, hotelId, from, to, page, pageSize);
        return Ok(result);
    }

    [HttpPut("reservations/{id}/status")]
    public async Task<ActionResult<ReservationDto>> UpdateReservationStatus(int id, [FromBody] AdminUpdateReservationStatusDto dto)
    {
        var result = await _adminService.UpdateReservationStatusAsync(id, dto.Status);
        return Ok(result);
    }

    [HttpPut("reservations/{id}/confirm")]
    public async Task<ActionResult<ReservationDto>> ConfirmReservation(int id, [FromBody] AdminConfirmReservationDto dto)
    {
        var result = await _adminService.ConfirmReservationAsync(id, dto.Confirm);
        return Ok(result);
    }

    [HttpGet("logs")]
    public async Task<ActionResult<PagedResponseDto<LogDto>>> GetLogs([FromQuery] LogFilterDto filter)
    {
        var result = await _adminService.GetLogsAsync(filter);

        if (TryGetCurrentUserId(out var currentAdminId))
            await LogInfoAsync("Admin log listesi goruntulendi.", userId: currentAdminId);

        return Ok(result);
    }

    [HttpGet("logs/{id}")]
    public async Task<ActionResult<LogDto>> GetLogById(int id)
    {
        var log = await _adminService.GetLogByIdAsync(id);
        if (log == null)
            return NotFound();

        if (TryGetCurrentUserId(out var currentAdminId))
            await LogInfoAsync($"Admin log detayi goruntulendi: LogId={id}", userId: currentAdminId);

        return Ok(log);
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId);
    }

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
