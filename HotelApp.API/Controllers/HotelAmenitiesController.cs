using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/amenities")]

// Otel ile olanak arasındaki ilişkiyi yöneten API controller'dır.
// Otele olanak atama ve otelden olanak kaldırma işlemleri burada yapılır.
// Bu işlemler sadece Admin rolündeki kullanıcılar tarafından kullanılabilir.
public class HotelAmenitiesController : ControllerBase
{
    private readonly IAmenityService _service;

    // Olanak atama ve kaldırma işlemleri için servis bağımlılığı constructor ile alınır
    public HotelAmenitiesController(IAmenityService service)
    {
        _service = service;
    }

    // E34 - POST
    // Belirtilen otele birden fazla olanak atar.
    // AmenityIds listesi request body içinden alınır.
    // Servis sonucunda eklenen ve bulunamayan olanak bilgileri döndürülür.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAmenities(int hotelId, [FromBody] HotelAmenityAssignDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.AddAmenitiesToHotelAsync(hotelId, dto.AmenityIds);

        return Ok(result);
    }

    // E35 - DELETE
    // Belirtilen otelden seçilen olanağı kaldırır.
    // hotelId hangi otelden silineceğini, amenityId ise hangi olanağın kaldırılacağını belirtir.
    [HttpDelete("{amenityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveAmenity(int hotelId, int amenityId)
    {
        await _service.RemoveAmenityFromHotelAsync(hotelId, amenityId);
        return NoContent();
    }
}
