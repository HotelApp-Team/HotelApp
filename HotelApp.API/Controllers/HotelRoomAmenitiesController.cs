using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/rooms/{roomId}/amenities")]
public class HotelRoomAmenitiesController : ControllerBase
{
    private readonly IRoomAmenityService _service;

    public HotelRoomAmenitiesController(IRoomAmenityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByHotelRoom(int hotelId, int roomId)
    {
        var result = await _service.GetByHotelRoomAsync(hotelId, roomId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Assign(int hotelId, int roomId, [FromBody] HotelRoomAmenityAssignDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.SetHotelRoomAmenitiesAsync(hotelId, roomId, dto.RoomAmenityIds);
        return Ok(result);
    }

    [HttpDelete("{roomAmenityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int hotelId, int roomId, int roomAmenityId)
    {
        await _service.RemoveFromHotelRoomAsync(hotelId, roomId, roomAmenityId);
        return NoContent();
    }
}
