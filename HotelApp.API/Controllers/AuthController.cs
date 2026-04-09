using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;

namespace HotelApp.API.Controllers;

    [Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
}

