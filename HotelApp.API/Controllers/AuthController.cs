using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly ILoggerService _logger;

    public AuthController(IAuthService authService, IUserService userService, ILoggerService logger)
    {
        _authService = authService;
        _userService = userService;
        _logger = logger;
    }

    //POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        var result = await _authService.RegisterAsync(userRegisterDto);

        await LogInfoAsync(
            result.RequiresEmailVerification
                ? $"Yeni kullanıcı kaydı oluşturuldu, email doğrulaması bekleniyor: {userRegisterDto.Email}"
                : $"Yeni kullanıcı kaydı oluşturuldu: {userRegisterDto.Email}",
            userEmail: userRegisterDto.Email);

        return Ok(result);
    }

    //POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var result = await _authService.LoginAsync(userLoginDto);

        await LogInfoAsync(
            $"Kullanıcı giriş yaptı. RememberMe: {(userLoginDto.RememberMe ? "Evet" : "Hayır")}",
            userEmail: result.Email);

        return Ok(result);
    }

    // GET api/auth/profile
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized("Token içinde geçerli kullanıcı bilgisi bulunamadı.");

        var result = await _userService.GetProfileAsync(userId);
        return Ok(result);
    }

    // GET api/auth/verify-email?token=abc123
    // Email doğrulama linkinden gelen token'ı alır, service'e gönderir ve sonucu döndürür
    [AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        // Token query string'den gelir, doğrulama ve JWT üretme mantığı service tarafında çalışır
        var result = await _authService.VerifyEmailAsync(token);

        await LogInfoAsync(
            $"Email doğrulandı ve otomatik giriş yapıldı: {result.Email}",
            userEmail: result.Email);

        return Ok(result);
    }

    // POST api/auth/resend-verification
    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        var result = await _authService.ResendVerificationAsync(dto);

        await LogInfoAsync(
            $"Email doğrulama maili yeniden gönderildi: {dto.Email}",
            userEmail: dto.Email);

        return Ok(new { Message = result });
    }

    // PUT api/auth/profile
    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] ProfileDto profileDto)
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized("Token içinde geçerli kullanıcı bilgisi bulunamadı.");

        var result = await _userService.UpdateProfileAsync(userId, profileDto);

        await LogInfoAsync(
            $"Kullanıcı profilini güncelledi: {result.Email}",
            userId: result.Id,
            userEmail: result.Email);

        return Ok(result);
    }

    private async Task LogInfoAsync(string message, int? userId = null, string? userEmail = null)
    {
        await _logger.LogInfoAsync(
            message: message,
            userId: userId,
            userEmail: userEmail,
            controller: nameof(AuthController),
            action: $"{Request.Method} {Request.Path}",
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId);
    }
}
