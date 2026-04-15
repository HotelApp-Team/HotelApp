using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    //POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        var result = await _authService.RegisterAsync(userRegisterDto);
        return Ok(result);

    }

    //POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var result = await _authService.LoginAsync(userLoginDto);
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
        return Ok(result);
    }

    // POST api/auth/resend-verification
    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        var result = await _authService.ResendVerificationAsync(dto);
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
        return Ok(result);
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId);
    }
}

