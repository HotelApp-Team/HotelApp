using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// Olanak Yonetimi tek sayfa: Otel Olanaklari + Oda Olanaklari iki tab.
// Index sadece okuma yapar; ekleme/duzenleme/silme islemleri mevcut
// AmenityController ve RoomAmenityController action'larina post edilir.
[Route("Admin/AmenityMgmt")]
[ServiceFilter(typeof(AdminFilter))]
public class AmenityMgmtController : Controller
{
    private readonly ApiService _apiService;

    public AmenityMgmtController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        // Iki listeyi paralel cek; tab'larda gostermek icin yeterli veri.
        var amenityResponse = await _apiService.GetAsync<List<AmenityViewModel>>("amenities");
        var roomAmenityResponse = await _apiService.GetAsync<List<RoomAmenityViewModel>>("room-amenities");

        var model = new AdminAmenityMgmtPageViewModel
        {
            Amenities = amenityResponse.Success ? amenityResponse.Data ?? new() : new(),
            RoomAmenities = roomAmenityResponse.Success ? roomAmenityResponse.Data ?? new() : new()
        };

        // TempData anahtarlari _AdminLayout ile uyumlu: "Success" / "Error".
        // Bu anahtarlar admin controller standardi (RoomType, AdminUserMgmt vb. tum admin sayfalari ayni).
        if (!amenityResponse.Success)
            TempData["Error"] = amenityResponse.Message ?? "Otel olanaklari listesi alinamadi.";
        else if (!roomAmenityResponse.Success)
            TempData["Error"] = roomAmenityResponse.Message ?? "Oda olanaklari listesi alinamadi.";

        return View("~/Views/Admin/AmenityMgmt.cshtml", model);
    }
}
