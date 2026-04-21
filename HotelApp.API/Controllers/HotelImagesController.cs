using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/images")]
public class HotelImagesController : Controller
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.HotelId != hotelId)
            return BadRequest("HotelId uyumsuz.");

        await _service.AddAsync(dto);
        return Ok("Görsel eklendi.");
    }

    // E18 - PUT: /api/hotels/{id}/images/{imgId}/cover
    [HttpPut("{imgId}/cover")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetMainImage(int hotelId, int imgId)
    {
        await _service.SetMainImageAsync(hotelId, imgId);
        return Ok("Kapak görseli güncellendi.");
    }

    // E19 - DELETE: /api/hotels/{id}/images/{imgId}
    [HttpDelete("{imgId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(int imgId)
    {
        await _service.DeleteAsync(imgId);
        return Ok("Görsel silindi.");
    }
}
