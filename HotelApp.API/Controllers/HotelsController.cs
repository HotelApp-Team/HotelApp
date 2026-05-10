using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ILoggerService _logger;

    public HotelsController(IHotelService hotelService, ILoggerService logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    // E04 - GET /api/hotels (Pagination)
    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] string? city,
    [FromQuery] byte? star,
    [FromQuery] string? sortBy = "name",
    [FromQuery] string? sortOrder = "asc",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        if (page < 1)
            return BadRequest(new { Message = "Page 1 veya daha büyük olmalıdır" });

        if (pageSize < 1 || pageSize > 100)
            return BadRequest(new { Message = "PageSize 1 ile 100 arasında olmalıdır" });

        var result = await _hotelService.GetAllFilteredAsync(
            city, star, sortBy, sortOrder, page, pageSize);

        return Ok(result);
    }

    // E05 - GET /api/hotels/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);

        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    // E06 - GET /api/hotels/cities
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _hotelService.GetCitiesAsync();
        return Ok(cities);
    }

    // E52 - GET /api/hotels/featured-cities : Anasayfada gosterilecek one cikan sehir kartlarini getirir.
    [HttpGet("featured-cities")]
    public async Task<IActionResult> GetFeaturedCities([FromQuery] int count = 5)
    {
        var cities = await _hotelService.GetFeaturedCitiesAsync(count);
        return Ok(cities);
    }

    // E07 - GET /api/hotels/search
    [HttpGet("search")]
    public async Task<IActionResult> Search(
    [FromQuery] HotelSearchRequestDto dto,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _hotelService.SearchAsync(dto, page, pageSize);

            await LogInfoAsync(
                $"Otel arama yapıldı: City={dto.City}, Star={dto.StarRating}");

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                Message = ex.Message
            });
        }
    }

    // E08 - POST /api/hotels
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] HotelCreateDto dto)
    {
        await _hotelService.CreateAsync(dto);

        await LogInfoAsync(
            $"Yeni otel oluşturuldu: Name={dto.Name}, City={dto.City}");

        return StatusCode(201);
    }

    // E09 - PUT /api/hotels
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] HotelUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id uyumsuz");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _hotelService.UpdateAsync(dto);

        await LogInfoAsync(
            $"Otel güncellendi: HotelId={dto.Id}");

        return NoContent();
    }

    // E10 - DELETE /api/hotels/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _hotelService.DeleteAsync(id);

        await LogInfoAsync(
            $"Otel silindi: HotelId={id}");

        return NoContent();
    }

    // Ortak bilgi logu atmak icin kullanilir.
    // Mesaj, kullanici bilgisi, controller/action ve IP bilgisini tek yerden logger servisine yollar.
    private async Task LogInfoAsync(string message, int? userId = null, string? userEmail = null)
    {
        await _logger.LogInfoAsync(
            message: message,
            userId: userId,
            userEmail: userEmail,
            controller: nameof(HotelsController),
            action: $"{Request.Method} {Request.Path}",
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
    }
}
