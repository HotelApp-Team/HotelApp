using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// Oda olanaklari icin CRUD action'lari. Sayfa render'i artik
// AdminAmenitiesController/Index uzerinden tek sayfada yapiliyor.
// Bu controller sadece form post hedefi olarak duruyor.
[Route("Admin/RoomAmenities")]
[ServiceFilter(typeof(AdminFilter))]
public class RoomAmenityController : Controller
{
    private readonly ApiService _apiService;

    public RoomAmenityController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "CreateForm")] AdminRoomAmenityFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Oda olanağı bilgileri eksik veya hatalı.";
            return RedirectToAction("Index", "AdminAmenities");
        }

        var response = await _apiService.PostAsync<object>("room-amenities", new
        {
            model.Name,
            model.Icon
        });

        TempData[response.Success ? "Success" : "Error"] =
            response.Success ? "Oda olanağı eklendi." : response.Message ?? "Oda olanağı eklenemedi.";

        return RedirectToAction("Index", "AdminAmenities");
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminRoomAmenityFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Oda olanağı bilgileri eksik veya hatalı.";
            return RedirectToAction("Index", "AdminAmenities");
        }

        var response = await _apiService.PutAsync($"room-amenities/{id}", new
        {
            Id = id,
            model.Name,
            model.Icon
        });

        TempData[response.Success ? "Success" : "Error"] =
            response.Success ? "Oda olanağı güncellendi." : response.Message ?? "Oda olanağı güncellenemedi.";

        return RedirectToAction("Index", "AdminAmenities");
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _apiService.DeleteAsync($"room-amenities/{id}");

        TempData[response.Success ? "Success" : "Error"] =
            response.Success ? "Oda olanağı silindi." : response.Message ?? "Oda olanağı silinemedi.";

        return RedirectToAction("Index", "AdminAmenities");
    }
}
