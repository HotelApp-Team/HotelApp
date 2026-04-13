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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Token içinde kullanıcı bilgisi bulunamadı.");

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized("Geçersiz kullanıcı bilgisi.");

        var result = await _userService.GetProfileAsync(userId);
        return Ok(result);
    }
}

