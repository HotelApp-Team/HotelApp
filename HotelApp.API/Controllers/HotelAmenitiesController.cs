using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/amenities")]
public class HotelAmenitiesController : Controller
{
    private readonly IAmenityService _service;

    public HotelAmenitiesController(IAmenityService service)
    {
        _service = service;
    }

    // E34 - POST
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAmenity(int hotelId, [FromBody] HotelAmenityAssignDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.HotelId != hotelId)
            return BadRequest("HotelId uyumsuz.");

        await _service.AddAmenityToHotelAsync(dto.HotelId, dto.AmenityId);
        return Ok("Olanak otele eklendi.");
    }

    // E35 - DELETE
    [HttpDelete("{amenityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveAmenity(int hotelId, int amenityId)
    {
        await _service.RemoveAmenityFromHotelAsync(hotelId, amenityId);
        return Ok("Olanak kaldırıldı.");
    }
}
