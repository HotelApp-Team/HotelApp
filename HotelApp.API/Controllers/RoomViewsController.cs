using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

// Admin tarafinda oda formundaki manzara dropdown'ini doldurmak icin
// kullanilir. Sadece liste donen tek endpoint var, seed-only oldugu icin
// CRUD endpoint'leri yazmadik.
[Route("api/[controller]")]
[ApiController]
public class RoomViewsController : ControllerBase
{
    private readonly IRoomViewService _service;

    public RoomViewsController(IRoomViewService service)
    {
        _service = service;
    }

    // Tum aktif manzara turlerini DisplayOrder sirali dondurur.
    // Yetki: yalnizca admin (form dropdown'inda kullaniliyor).
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var views = await _service.GetAllAsync();
        return Ok(views);
    }
}
