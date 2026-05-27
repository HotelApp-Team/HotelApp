using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/room-amenities")]
public class RoomAmenitiesController : ControllerBase
{
    private readonly IRoomAmenityService _service;

    public RoomAmenitiesController(IRoomAmenityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] RoomAmenityCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.AddAsync(dto);
        return StatusCode(201);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] RoomAmenityUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id uyumsuz.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
