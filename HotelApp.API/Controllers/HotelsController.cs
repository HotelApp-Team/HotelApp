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
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var hotels = await _hotelService.GetAllAsync();

        var totalCount = hotels.Count();

        var paged = hotels
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return Ok(new
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = paged
        });
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
    public async Task<IActionResult> GetByCity([FromQuery] string city)
    {
        var hotels = await _hotelService.GetByCityAsync(city);
        return Ok(hotels);
    }


    // E07 - GET /api/hotels/search
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? city,
        [FromQuery] byte? star,
        [FromQuery] decimal? minRating,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var hotels = await _hotelService.GetAllAsync();

        // Filtering
        if (!string.IsNullOrEmpty(city))
            hotels = hotels.Where(x => x.City.ToLower() == city.ToLower());

        if (star.HasValue)
            hotels = hotels.Where(x => x.StarRating == star);

        if (minRating.HasValue)
            hotels = hotels.Where(x => x.AvgRating >= minRating);

        // Total count (filter sonrası)
        var totalCount = hotels.Count();

        // Pagination
        var pagedData = hotels
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = pagedData
        });
    }
   
    // E08 - POST /api/hotels
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] HotelCreateDto dto)
    {
        await _hotelService.CreateAsync(dto);
        return Ok("Otel oluşturuldu");
    }

    // E09 - PUT /api/hotels
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] HotelUpdateDto dto)
    {
        await _hotelService.UpdateAsync(dto);
        return Ok("Otel güncellendi");
    }

    // E10 - DELETE /api/hotels/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _hotelService.DeleteAsync(id);
        return Ok("Otel silindi (soft delete)");
    }
}
