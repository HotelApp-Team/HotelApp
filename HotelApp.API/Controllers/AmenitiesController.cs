using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/amenities")]

// Otel olanakları için API endpointlerini yöneten controller'dır.
// Wi-Fi, havuz, otopark, spa gibi otellere atanabilecek ortak olanak kayıtları bu controller üzerinden yönetilir.
// Listeleme işlemi herkese açıkken, oluşturma, güncelleme ve silme işlemleri sadece Admin rolündeki kullanıcılara açıktır.
public class AmenitiesController : ControllerBase
{
    private readonly IAmenityService _service;

    // Olanak işlemlerinde kullanılacak servis constructor üzerinden alınır.
    public AmenitiesController(IAmenityService service)
    {
        _service = service;
    }

    // E32 - GET
    // Sistemde kayıtlı tüm otel olanaklarını listeler.
    // Bu endpoint otel detay sayfalarında olanakları göstermek veya admin panelinde olanak listesini yönetmek için kullanılabilir.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // E33 - POST
    // Yeni bir otel olanağı oluşturur.
    // Request body içinden gelen AmenityCreateDto ile olanak adı ve ikon bilgisi alınır.
    // ModelState geçersizse işlem servise gitmeden BadRequest döndürülür.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] AmenityCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.AddAsync(dto);
        return StatusCode(201);
    }

    // E49 - PUT
    // Var olan bir otel olanağını günceller.
    // Route üzerinden gelen id ile request body içindeki dto.Id aynı olmalıdır.
    // Bu kontrol yanlış kaydın güncellenmesini engeller.
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] AmenityUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id uyumsuz.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.UpdateAsync(dto);
        return NoContent();
    }

    // E50 - DELETE
    // Id değerine göre bir otel olanağını siler.
    // Olanak herhangi bir otelde kullanılıyorsa silme işlemi servis katmanında engellenir.
    // İşlem başarılı olursa 204 NoContent cevabı döndürülür.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
