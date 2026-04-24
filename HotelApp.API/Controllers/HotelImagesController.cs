using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/images")]
public class HotelImagesController : ControllerBase
{
    private readonly IHotelImageService _service;

    public HotelImagesController(IHotelImageService service)
    {
        _service = service;
    }

    // E16 - GET: /api/hotels/{id}/images
    [HttpGet]
    public async Task<IActionResult> GetImages(int hotelId)
    {
        var images = await _service.GetByHotelIdAsync(hotelId);
        return Ok(images);
    }

    // E17 - POST: /api/hotels/{id}/images
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImage(int hotelId, [FromBody] HotelImageCreateDto dto)
    {
        await _service.AddAsync(hotelId, dto);
        return StatusCode(201);
    }

    // E18 - PUT: /api/hotels/{id}/images/{imgId}/cover
    [HttpPut("{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateImage(int hotelId, int imageId, [FromBody] HotelImageUpdateDto dto)
    {
        if (imageId != dto.Id)
            return BadRequest("Id uyumsuz");

        await _service.UpdateAsync(hotelId, dto);

        return NoContent();
    }

    // E19 - DELETE: /api/hotels/{id}/images/{imgId}
    [HttpDelete("{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(int hotelId, int imageId)
    {
        await _service.DeleteAsync(hotelId, imageId);
        return NoContent();
    }
}
