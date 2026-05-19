using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

[Route("Admin/RoomAmenities")]
[ServiceFilter(typeof(AdminFilter))]
public class RoomAmenityController : Controller
{
    private readonly ApiService _apiService;

    public RoomAmenityController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var model = await BuildPageModelAsync();
        return View("~/Views/Admin/RoomAmenities.cshtml", model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "CreateForm")] AdminRoomAmenityFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var pageModel = await BuildPageModelAsync();
            pageModel.CreateForm = model;
            return View("~/Views/Admin/RoomAmenities.cshtml", pageModel);
        }

        var response = await _apiService.PostAsync<object>("room-amenities", new
        {
            model.Name,
            model.Icon
        });

        if (!response.Success)
        {
            var pageModel = await BuildPageModelAsync();
            pageModel.CreateForm = model;
            TempData["ErrorMessage"] = response.Message ?? "Oda olanağı eklenemedi.";
            return View("~/Views/Admin/RoomAmenities.cshtml", pageModel);
        }

        TempData["SuccessMessage"] = "Oda olanağı eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminRoomAmenityFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var pageModel = await BuildPageModelAsync();
            pageModel.EditForm = model;
            return View("~/Views/Admin/RoomAmenities.cshtml", pageModel);
        }

        var response = await _apiService.PutAsync($"room-amenities/{id}", new
        {
            Id = id,
            model.Name,
            model.Icon
        });

        if (!response.Success)
        {
            var pageModel = await BuildPageModelAsync();
            pageModel.EditForm = model;
            TempData["ErrorMessage"] = response.Message ?? "Oda olanağı güncellenemedi.";
            return View("~/Views/Admin/RoomAmenities.cshtml", pageModel);
        }

        TempData["SuccessMessage"] = "Oda olanağı güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _apiService.DeleteAsync($"room-amenities/{id}");

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Oda olanağı silindi." : response.Message ?? "Oda olanağı silinemedi.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<AdminRoomAmenityPageViewModel> BuildPageModelAsync()
    {
        var response = await _apiService.GetAsync<List<RoomAmenityViewModel>>("room-amenities");

        if (!response.Success)
            TempData["ErrorMessage"] = response.Message ?? "Oda olanağı listesi alınamadı.";

        return new AdminRoomAmenityPageViewModel
        {
            RoomAmenities = response.Success ? response.Data ?? new() : new(),
            CreateForm = new AdminRoomAmenityFormViewModel(),
            EditForm = new AdminRoomAmenityFormViewModel()
        };
    }
}
