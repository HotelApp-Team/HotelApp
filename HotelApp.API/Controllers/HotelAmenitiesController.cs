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
    public async Task<IActionResult> AddAmenities(int hotelId, [FromBody] HotelAmenityAssignDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.AddAmenitiesToHotelAsync(hotelId, dto.AmenityIds);

        return NoContent();
    }

    // E35 - DELETE
    [HttpDelete("{amenityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveAmenity(int hotelId, int amenityId)
    {
        try
        {
            await _service.RemoveAmenityFromHotelAsync(hotelId, amenityId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
