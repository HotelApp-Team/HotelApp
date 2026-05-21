using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// Otel olanaklari icin CRUD action'lari. Sayfa render'i artik
// AmenityMgmtController/Index uzerinden tek sayfada yapiliyor.
// Bu controller sadece form post hedefi olarak duruyor.
[Route("Admin/Amenities")]
[ServiceFilter(typeof(AdminFilter))]
public class AmenityController : Controller
{
    private readonly ApiService _apiService;

    public AmenityController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        // Eski URL'i hala yazan biri olursa yeni birlesik sayfaya gonder.
        return RedirectToAction("Index", "AmenityMgmt");
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "CreateForm")] AdminAmenityFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Olanak bilgileri eksik veya hatalı.";
            return RedirectToAction("Index", "AmenityMgmt");
        }

        var response = await _apiService.PostAsync<object>("amenities", new
        {
            model.Name,
            model.Icon
        });

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Olanak başarıyla eklendi." : response.Message ?? "Olanak eklenemedi.";

        return RedirectToAction("Index", "AmenityMgmt");
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminAmenityFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Olanak bilgileri eksik veya hatalı.";
            return RedirectToAction("Index", "AmenityMgmt");
        }

        var response = await _apiService.PutAsync($"amenities/{id}", new
        {
            Id = id,
            model.Name,
            model.Icon
        });

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Olanak başarıyla güncellendi." : response.Message ?? "Olanak güncellenemedi.";

        return RedirectToAction("Index", "AmenityMgmt");
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _apiService.DeleteAsync($"amenities/{id}");

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Olanak başarıyla silindi." : response.Message ?? "Olanak silinemedi.";

        return RedirectToAction("Index", "AmenityMgmt");
    }
}
