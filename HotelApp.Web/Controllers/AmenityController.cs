using HotelApp.Web.Filters;
using HotelApp.Web.Helpers;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

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
    public async Task<IActionResult> Index()
    {
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Auth");

        var response = await _apiService.GetAsync<List<AmenityViewModel>>("amenities");

        var model = new AdminAmenityPageViewModel
        {
            Amenities = response.Success ? response.Data ?? new() : new(),
            CreateForm = new AdminAmenityFormViewModel(),
            EditForm = new AdminAmenityFormViewModel()
        };

        if (!response.Success)
            TempData["ErrorMessage"] = response.Message ?? "Olanak listesi alınamadı.";

        return View("~/Views/Admin/Amenities.cshtml", model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminAmenityFormViewModel model)
    {
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            var pageModel = await BuildAmenityPageModelAsync();
            pageModel.CreateForm = model;
            return View("~/Views/Admin/Amenities.cshtml", pageModel);
        }

        var response = await _apiService.PostAsync<object>("amenities", new
        {
            model.Name,
            model.Icon
        });

        if (!response.Success)
        {
            var pageModel = await BuildAmenityPageModelAsync();
            pageModel.CreateForm = model;
            TempData["ErrorMessage"] = response.Message ?? "Olanak eklenemedi.";
            return View("~/Views/Admin/Amenities.cshtml", pageModel);
        }

        TempData["SuccessMessage"] = "Olanak başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminAmenityFormViewModel model)
    {
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            var pageModel = await BuildAmenityPageModelAsync();
            pageModel.EditForm = model;
            return View("~/Views/Admin/Amenities.cshtml", pageModel);
        }

        var response = await _apiService.PutAsync($"amenities/{id}", new
        {
            Id = id,
            model.Name,
            model.Icon
        });

        if (!response.Success)
        {
            var pageModel = await BuildAmenityPageModelAsync();
            pageModel.EditForm = model;
            TempData["ErrorMessage"] = response.Message ?? "Olanak güncellenemedi.";
            return View("~/Views/Admin/Amenities.cshtml", pageModel);
        }

        TempData["SuccessMessage"] = "Olanak başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Auth");

        var response = await _apiService.DeleteAsync($"amenities/{id}");

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Olanak başarıyla silindi." : response.Message ?? "Olanak silinemedi.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<AdminAmenityPageViewModel> BuildAmenityPageModelAsync()
    {
        var response = await _apiService.GetAsync<List<AmenityViewModel>>("amenities");

        return new AdminAmenityPageViewModel
        {
            Amenities = response.Success ? response.Data ?? new() : new(),
            CreateForm = new AdminAmenityFormViewModel(),
            EditForm = new AdminAmenityFormViewModel()
        };
    }
}
