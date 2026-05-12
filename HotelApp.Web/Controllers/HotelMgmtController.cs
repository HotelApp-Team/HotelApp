using HotelApp.Web.Filters;
using HotelApp.Web.Helpers;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

[Route("Admin/Hotels")]
[ServiceFilter(typeof(AdminFilter))]
public class HotelMgmtController : Controller
{
    private readonly ApiService _apiService;

    public HotelMgmtController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? city,
        byte? starRating,
        string sortBy = "name",
        string sortOrder = "asc",
        int page = 1,
        int pageSize = 10)
    {
        var query = $"hotels?city={Uri.EscapeDataString(city ?? string.Empty)}" +
                    $"&star={starRating}" +
                    $"&sortBy={Uri.EscapeDataString(sortBy)}" +
                    $"&sortOrder={Uri.EscapeDataString(sortOrder)}" +
                    $"&page={page}&pageSize={pageSize}";

        var response = await _apiService.GetAsync<PagedHotelListResponse>(query);

        var model = new AdminHotelListPageViewModel
        {
            City = city,
            StarRating = starRating,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            PageSize = pageSize,
            Hotels = response.Success ? response.Data?.Data ?? new() : new(),
            TotalCount = response.Success ? response.Data?.TotalCount ?? 0 : 0
        };

        if (!response.Success)
            TempData["ErrorMessage"] = response.Message ?? "Otel listesi alınamadı.";

        return View("~/Views/Admin/Hotels.cshtml", model);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var model = new AdminHotelFormViewModel();
        await LoadAmenitiesAsync(model);

        return View("~/Views/Admin/HotelEdit.cshtml", model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminHotelFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadAmenitiesAsync(model);
            return View("~/Views/Admin/HotelEdit.cshtml", model);
        }

        var dto = new
        {
            model.Name,
            model.Description,
            model.Address,
            model.City,
            model.Country,
            model.Phone,
            model.Email,
            model.Latitude,
            model.Longitude,
            model.StarRating
        };

        var response = await _apiService.PostAsync<object>("hotels", dto);

        if (!response.Success)
        {
            TempData["ErrorMessage"] = response.Message ?? "Otel eklenemedi.";
            await LoadAmenitiesAsync(model);
            return View("~/Views/Admin/HotelEdit.cshtml", model);
        }

        TempData["SuccessMessage"] = "Otel başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await BuildHotelFormModelAsync(id);

        if (model == null)
        {
            TempData["ErrorMessage"] = "Otel bilgileri alınamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Admin/HotelEdit.cshtml", model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminHotelFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadAmenitiesAsync(model, id);
            return View("~/Views/Admin/HotelEdit.cshtml", model);
        }

        var dto = new
        {
            Id = id,
            model.Name,
            model.Description,
            model.Address,
            model.City,
            model.Country,
            model.Phone,
            model.Email,
            model.Latitude,
            model.Longitude,
            model.StarRating,
            model.IsActive
        };

        var response = await _apiService.PutAsync($"hotels/{id}", dto);

        if (!response.Success)
        {
            TempData["ErrorMessage"] = response.Message ?? "Otel güncellenemedi.";
            await LoadAmenitiesAsync(model, id);
            return View("~/Views/Admin/HotelEdit.cshtml", model);
        }

        TempData["SuccessMessage"] = "Otel bilgileri güncellendi.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _apiService.DeleteAsync($"hotels/{id}");

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Otel silindi." : response.Message ?? "Otel silinemedi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{hotelId}/Images")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(int hotelId, HotelImageAdminFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Görsel bilgileri eksik.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        var response = await _apiService.PostAsync<object>($"hotels/{hotelId}/images", new
        {
            model.ImageUrl,
            model.IsMain,
            model.DisplayOrder
        });

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Görsel eklendi." : response.Message ?? "Görsel eklenemedi.";

        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Images/{imageId}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int hotelId, int imageId)
    {
        var response = await _apiService.DeleteAsync($"hotels/{hotelId}/images/{imageId}");

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Görsel silindi." : response.Message ?? "Görsel silinemedi.";

        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Images/{imageId}/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateImage(
        int hotelId,
        int imageId,
        HotelImageAdminFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Görsel bilgileri eksik veya hatalı.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        var response = await _apiService.PutAsync(
            $"hotels/{hotelId}/images/{imageId}",
            new
            {
                Id = imageId,
                model.ImageUrl,
                model.IsMain,
                model.DisplayOrder
            });

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success
                ? "Görsel güncellendi."
                : response.Message ?? "Görsel güncellenemedi.";

        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Amenities")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignAmenities(int hotelId, List<int> amenityIds)
    {
        amenityIds ??= new List<int>();

        var hotelResponse = await _apiService.GetAsync<HotelDetailPageVm>($"hotels/{hotelId}");
        if (!hotelResponse.Success || hotelResponse.Data == null)
        {
            TempData["ErrorMessage"] = "Otel olanak bilgileri alınamadı.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        var existingIds = hotelResponse.Data.Amenities.Select(x => x.Id).ToList();
        var selectedIds = amenityIds.Distinct().ToList();

        var toAdd = selectedIds.Except(existingIds).ToList();
        var toRemove = existingIds.Except(selectedIds).ToList();

        if (toAdd.Any())
        {
            var addResponse = await _apiService.PostAsync<object>(
                $"hotels/{hotelId}/amenities",
                new { AmenityIds = toAdd });

            if (!addResponse.Success)
            {
                TempData["ErrorMessage"] = addResponse.Message ?? "Olanak eklenemedi.";
                return RedirectToAction(nameof(Edit), new { id = hotelId });
            }
        }

        foreach (var amenityId in toRemove)
        {
            var removeResponse = await _apiService.DeleteAsync($"hotels/{hotelId}/amenities/{amenityId}");
            if (!removeResponse.Success)
            {
                TempData["ErrorMessage"] = removeResponse.Message ?? "Bazı olanaklar kaldırılamadı.";
                return RedirectToAction(nameof(Edit), new { id = hotelId });
            }
        }

        TempData["SuccessMessage"] = "Olanaklar güncellendi.";
        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Amenities/{amenityId}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAmenity(int hotelId, int amenityId)
    {
        var response = await _apiService.DeleteAsync(
            $"hotels/{hotelId}/amenities/{amenityId}");

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success
                ? "Olanak kaldırıldı."
                : response.Message ?? "Olanak kaldırılamadı.";

        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    private async Task<AdminHotelFormViewModel?> BuildHotelFormModelAsync(int id)
    {
        var hotelResponse = await _apiService.GetAsync<HotelDetailPageVm>($"hotels/{id}");
        if (!hotelResponse.Success || hotelResponse.Data == null)
            return null;

        var hotel = hotelResponse.Data;

        var model = new AdminHotelFormViewModel
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            Phone = hotel.Phone,
            Email = hotel.Email,
            Latitude = hotel.Latitude,
            Longitude = hotel.Longitude,
            StarRating = hotel.StarRating,
            IsActive = hotel.IsActive,
            Images = hotel.Images,
            SelectedAmenityIds = hotel.Amenities.Select(x => x.Id).ToList()
        };

        await LoadAmenitiesAsync(model, id);
        return model;
    }

    private async Task LoadAmenitiesAsync(AdminHotelFormViewModel model, int hotelId = 0)
    {
        var amenityResponse = await _apiService.GetAsync<List<AmenityViewModel>>("amenities");

        if (!amenityResponse.Success)
        {
            model.AllAmenities = new();
            return;
        }

        model.AllAmenities = (amenityResponse.Data ?? new())
            .Select(x => new AdminAmenityItemViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Icon = x.Icon,
                IsAssigned = model.SelectedAmenityIds.Contains(x.Id)
            })
            .ToList();

        if (hotelId > 0 && model.Images.Count == 0)
        {
            var imageResponse = await _apiService.GetAsync<List<HotelImageViewModel>>($"hotels/{hotelId}/images");
            model.Images = imageResponse.Success ? imageResponse.Data ?? new() : new();
        }
    }
}