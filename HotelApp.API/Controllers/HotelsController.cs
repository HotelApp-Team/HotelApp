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

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
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
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 100)
            pageSize = 100;

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

    // E06 - GET /api/hotels/cities?city=Ankara
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _hotelService.GetCitiesAsync();
        return Ok(cities);
    }

    // E07 - GET /api/hotels/search
    [HttpGet("search")]
    public async Task<IActionResult> Search(
      [FromQuery] HotelSearchRequestDto dto,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10)
    {
        var result = await _hotelService.SearchAsync(dto, page, pageSize);
        return Ok(result);
    }

    // E08 - POST /api/hotels
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] HotelCreateDto dto)
    {
        await _hotelService.CreateAsync(dto);
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
        return NoContent();
    }
    // E10 - DELETE /api/hotels/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _hotelService.DeleteAsync(id);
        return NoContent();
    }
}
