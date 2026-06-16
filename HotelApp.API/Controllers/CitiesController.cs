using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

// Sehir (il) listesini sunar. Otel formundaki sehir dropdown'ini doldurmak
// icin kullanilir. Iller seed-only oldugu icin sadece liste donen tek endpoint var.
[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly ICityService _service;

    public CitiesController(ICityService service)
    {
        _service = service;
    }

    // Tum sehirleri ada gore sirali dondurur.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cities = await _service.GetAllAsync();
        return Ok(cities);
    }
}
