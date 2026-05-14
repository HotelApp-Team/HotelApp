using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.API.Controllers;

// AuthController sınıfının amacı, auth endpointlerini yönetmektir.
// Kayıt, giriş, profil, email doğrulama ve şifre değiştirme isteklerini ilgili servislere yönlendirir.
// SOLID: SRP kullanır; sadece auth HTTP isteklerinden sorumludur.
// SOLID: DIP kullanır; iş mantığı için IAuthService, IUserService ve ILoggerService arayüzlerine bağlıdır.

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

    // E01 - POST api/auth/register : Yeni kullanıcı kaydı oluşturur.
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        var result = await _authService.RegisterAsync(userRegisterDto);

        // Kayit basarili ama verify maili gidemediyse warning log atiyoruz.
        // Boylece kullaniciya kontrollu cevap donerken operasyonel kayit da tutuluyor.
        if (result.RequiresEmailVerification && !result.VerificationEmailSent)
        {
            await _logger.LogWarningAsync(
                message: $"Kullanıcı kaydı oluşturuldu fakat doğrulama maili gönderilemedi: {userRegisterDto.Email}",
                userEmail: userRegisterDto.Email,
                controller: nameof(AuthController),
                action: $"{Request.Method} {Request.Path}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
        }
        else
        {
            await _logger.LogInfoAsync(
                message: result.RequiresEmailVerification
                    ? $"Yeni kullanıcı kaydı oluşturuldu, email doğrulaması bekleniyor: {userRegisterDto.Email}"
                    : $"Yeni kullanıcı kaydı oluşturuldu: {userRegisterDto.Email}",
                userEmail: userRegisterDto.Email,
                controller: nameof(AuthController),
                action: $"{Request.Method} {Request.Path}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
        }

        return Ok(result);
    }

    // E02 - POST api/auth/login : Kullanıcı girişi yapar ve JWT token döndürür.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var result = await _authService.LoginAsync(userLoginDto);

        await LogInfoAsync(
            $"Kullanıcı giriş yaptı.",
            userEmail: result.Email);

        return Ok(result);
    }

    // E03 - GET api/auth/profile : Kullanıcının profil bilgilerini getirir.
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized("Token içinde geçerli kullanıcı bilgisi bulunamadı.");

        var result = await _userService.GetProfileAsync(userId);
        return Ok(result);
    }

    // E42 - GET api/auth/verify-email?token={token} : Email doğrulama token'ını kontrol eder ve otomatik giriş sağlar.
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

    // E43 - POST api/auth/resend-verification : Doğrulama mailinin yeniden gönderilmesini sağlar.
    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        var result = await _authService.ResendVerificationAsync(dto);

        // Disariya her durumda generic mesaj donuyoruz.
        // Ic tarafta ise mailin denenip denenmedigine ve sonucuna gore uygun log atiyoruz.
        if (result.EmailSendAttempted && !result.EmailSent)
        {
            await _logger.LogWarningAsync(
                message: $"Doğrulama maili yeniden gönderme denendi fakat mail gönderilemedi: {dto.Email}",
                userEmail: dto.Email,
                controller: nameof(AuthController),
                action: $"{Request.Method} {Request.Path}",
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
        }
        else if (result.EmailSendAttempted)
        {
            await LogInfoAsync(
                $"Email doğrulama maili yeniden gönderildi: {dto.Email}",
                userEmail: dto.Email);
        }
        else
        {
            await LogInfoAsync(
                $"Email doğrulama maili yeniden gönderme isteği işlendi: {dto.Email}",
                userEmail: dto.Email);
        }

        return Ok(new { Message = result.Message });
    }

    // E54 - POST api/auth/forgot-password : Kullanıcıya şifre yenileme linki gönderilmesini sağlar.
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var result = await _authService.ForgotPasswordAsync(dto);

        // Kullanici var/yok bilgisi disariya acik edilmez; log mesaji da genel tutulur.
        await LogInfoAsync(
            result.EmailSendAttempted
                ? $"Şifre yenileme maili gönderildi: {dto.Email}"
                : $"Şifre yenileme isteği işlendi: {dto.Email}",
            userEmail: dto.Email);

        return Ok(new { Message = result.Message });
    }

    // E55 - POST api/auth/reset-password : Reset token ile kullanıcının yeni şifre belirlemesini sağlar.
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _authService.ResetPasswordAsync(dto);

        await LogInfoAsync(
            $"Kullanıcı şifresini reset token ile yeniledi: {result.Email}",
            userEmail: result.Email);

        return Ok(new { Message = result.Message });
    }

    // E46 - PUT api/auth/profile : Kullanıcının profil bilgilerini güncellemesini sağlar.
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

    // Ortak bilgi logu atmak icin kullanilir.
    // Mesaj, kullanici bilgisi, controller/action ve IP bilgisini tek yerden logger servisine yollar.
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

    // Token icindeki kullanici id bilgisini okumayi dener.
    // Id bulunur ve int'e cevrilirse true doner, bulunamazsa false doner.
    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId);
    }

    // E47 - PUT api/auth/change-password : Kullanıcının şifresini değiştirmesini sağlar.
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto dto)
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized("Token içinde geçerli kullanıcı bilgisi bulunamadı.");
        
        await _userService.ChangePasswordAsync(userId, dto);

        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        await LogInfoAsync(
            $"Kullanıcı şifresini değiştirdi: UserId:{userId}",
            userId: userId,
            userEmail: userEmail);

        return Ok(new { Message = "Şifre başarıyla değiştirildi." });
    }
}
