using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/amenities")]
public class AmenitiesController : Controller
{
    private readonly IAmenityService _service;

    public AmenitiesController(IAmenityService service)
    {
        _service = service;
    }

    // E32 - GET
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // E33 - POST
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAmenityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.AddAsync(dto);
        return Ok("Olanak eklendi.");
    }

    // E49 - PUT
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAmenityDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id uyumsuz.");

        await _service.UpdateAsync(dto);
        return Ok("Olanak güncellendi.");
    }

    // E50 - DELETE
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok("Olanak silindi.");
    }
}
