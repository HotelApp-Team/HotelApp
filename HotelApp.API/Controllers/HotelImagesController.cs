using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId}/images")]

// Otellere ait görseller için API endpointlerini yönetir.
// Otel detay sayfasında görsel listeleme, admin panelinde görsel ekleme,
// güncelleme ve silme işlemleri bu controller üzerinden yapılır.
public class HotelImagesController : ControllerBase
{
    private readonly IHotelImageService _service;

    // Otel görsel işlemleri için servis bağımlılığı constructor ile alınır.
    public HotelImagesController(IHotelImageService service)
    {
        _service = service;
    }

    // E16 - GET: /api/hotels/{id}/images
    // Route üzerinden gelen hotelId değerine göre otele ait görselleri getirir.
    // Dönen liste otel detay sayfasındaki galeri veya admin görsel listesi için kullanılabilir
    [HttpGet]
    public async Task<IActionResult> GetImages(int hotelId)
    {
        var images = await _service.GetByHotelIdAsync(hotelId);
        return Ok(images);
    }

    // E17 - POST: /api/hotels/{id}/images
    // Route üzerinden gelen hotelId değerine göre ilgili otele yeni görsel ekler.
    // Görsel URL, ana görsel bilgisi ve sıralama değeri request body içinden alınır.
    // Bu işlem sadece Admin rolündeki kullanıcılar tarafından yapılabilir.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddImage(int hotelId, [FromBody] HotelImageCreateDto dto)
    {
        await _service.AddAsync(hotelId, dto);
        return StatusCode(201);
    }

    // E18 - PUT: /api/hotels/{hotelId}/images
    // Belirtilen otele ait tüm görsellerin sıralama ve ana görsel ayarlarını günceller.
    // İstek mevcut tüm görselleri içermeli ve tam olarak bir ana görsel seçilmelidir.
    // Güncelleme başarılı olduğunda 204 NoContent cevabı döner.
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateImage(int hotelId, [FromBody] HotelImageUpdateDto dto)
    {
        await _service.UpdateAsync(hotelId, dto);
        return NoContent();
    }

    // E19 - DELETE: /api/hotels/{id}/images/{imgId}
    // Route üzerinden gelen hotelId ve imageId değerlerine göre görsel siler.
    // Servis katmanı görselin gerçekten ilgili otele ait olup olmadığını kontrol eder.
    // Silme işlemi sadece Admin rolündeki kullanıcılar tarafından yapılabilir.
    [HttpDelete("{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(int hotelId, int imageId)
    {
        await _service.DeleteAsync(hotelId, imageId);
        return NoContent();
    }
}
