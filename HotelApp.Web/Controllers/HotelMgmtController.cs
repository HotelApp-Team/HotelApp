using HotelApp.Web.Filters;
using HotelApp.Web.Helpers;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
        bool? isActive,
        string sortBy = "name",
        string sortOrder = "asc",
        int page = 1,
        int pageSize = 10)
    {
        var query = $"hotels?city={Uri.EscapeDataString(city ?? string.Empty)}" +
            $"&star={starRating}" +
            $"&isActive={isActive}" +
            $"&sortBy={Uri.EscapeDataString(sortBy)}" +
            $"&sortOrder={Uri.EscapeDataString(sortOrder)}" +
            $"&page={page}&pageSize={pageSize}";

        var response = await _apiService.GetAsync<PagedHotelListResponse>(query);

        var model = new AdminHotelListPageViewModel
        {
            City = city,
            StarRating = starRating,
            IsActive = isActive,
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
        await LoadFormDependenciesAsync(model);

        return View("~/Views/Admin/HotelEdit.cshtml", model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminHotelFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadFormDependenciesAsync(model);
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

        var response = await _apiService.PostAsync<CreatedHotelResponseViewModel>("hotels", dto);

        if (!response.Success || response.Data == null)
        {
            TempData["ErrorMessage"] = response.Message ?? "Otel eklenemedi.";
            await LoadFormDependenciesAsync(model);
            return View("~/Views/Admin/HotelEdit.cshtml", model);
        }

        TempData["SuccessMessage"] = "Otel oluşturuldu. Şimdi olanak, görsel ve oda ekleyebilirsiniz.";
        return RedirectToAction(nameof(Edit), new { id = response.Data.Id });
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
            await LoadFormDependenciesAsync(model, id);
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
            await LoadFormDependenciesAsync(model, id);
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

    [HttpPost("{hotelId}/Rooms/{roomId}/Amenities")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoomAmenities(int hotelId, int roomId, List<int> roomAmenityIds)
    {
        roomAmenityIds ??= new List<int>();

        var response = await _apiService.PostAsync<object>(
            $"hotels/{hotelId}/rooms/{roomId}/amenities",
            new { RoomAmenityIds = roomAmenityIds.Distinct().ToList() });

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success
                ? "Oda olanakları güncellendi."
                : response.Message ?? "Oda olanakları güncellenemedi.";

        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Rooms")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRoom(int hotelId, AdminHotelRoomFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Oda bilgileri eksik veya hatalı.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        if (!TryParsePrices(hotelId, out var pricePerNight, out var extraGuestPricePerNight, out var errorResult))
        {
            return errorResult!;
        }

        var response = await _apiService.PostAsync<object>($"hotels/{hotelId}/rooms", new
        {
            model.RoomTypeId,
            PricePerNight = pricePerNight,
            ExtraGuestPricePerNight = extraGuestPricePerNight,
            model.TotalRooms,
            model.RoomViewId
        });

        if (!response.Success)
        {
            TempData["ErrorMessage"] = response.Message ?? "Oda eklenemedi.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        if (!string.IsNullOrWhiteSpace(model.ImageUrl))
        {
            var roomsResponse=await _apiService.GetAsync<List<AdminHotelRoomViewModel>>($"hotels/{hotelId}/rooms");

            var addedRoom = roomsResponse.Data?
                .FirstOrDefault(x => x.HotelId == hotelId && x.RoomTypeId == model.RoomTypeId);

            if (addedRoom == null)
            {
                TempData["ErrorMessage"] = "Oda eklendi fakat oda kaydı tekrar bulunamadığı için görsel eklenemedi.";
                return RedirectToAction(nameof(Edit), new { id = hotelId });
            }

            var imageResponse = await _apiService.PostAsync<object>($"rooms/{addedRoom.Id}/images", new
            {
                ImageUrl = model.ImageUrl,
                IsMain = model.IsMain,
                DisplayOrder = model.DisplayOrder
            });

            if (!imageResponse.Success)
            {
                TempData["ErrorMessage"] = imageResponse.Message ?? "Oda eklendi fakat görsel eklenemedi.";
                return RedirectToAction(nameof(Edit), new { id = hotelId });
            }
        }

        TempData["SuccessMessage"] = "Oda başarıyla eklendi.";
        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Rooms/{roomId}/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoom(int hotelId, int roomId, AdminHotelRoomViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Oda bilgileri eksik veya hatalı.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        if (!TryParsePrices(hotelId, out var pricePerNight, out var extraGuestPricePerNight, out var errorResult))
        {
            return errorResult!;
        }

        var isActiveRaw = Request.Form["IsActive"].ToString();

        if (!bool.TryParse(isActiveRaw, out var isActive))
        {
            TempData["ErrorMessage"] = "Oda durumu hatalı gönderildi.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        var response = await _apiService.PutAsync(
            $"hotels/{hotelId}/rooms/{roomId}",
            new
            {
                Id = roomId,
                model.RoomTypeId,
                PricePerNight = pricePerNight,
                ExtraGuestPricePerNight = extraGuestPricePerNight,
                model.TotalRooms,
                IsActive = isActive,
                model.RoomViewId
            });

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success
                ? "Oda bilgileri güncellendi."
                : response.Message ?? "Oda güncellenemedi.";

        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Rooms/{roomId}/Images")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRoomImage(int hotelId, int roomId, HotelImageAdminFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Görsel bilgileri eksik veya hatalı.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        var response = await _apiService.PostAsync<object>($"rooms/{roomId}/images", new
        {
            model.ImageUrl,
            model.IsMain,
            model.DisplayOrder
        });

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Oda görseli eklendi." : response.Message ?? "Oda görseli eklenemedi.";

        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Rooms/{roomId}/Images/{imageId}/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoomImage(int hotelId, int roomId, int imageId, HotelImageAdminFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Görsel bilgileri eksik veya hatalı.";
            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        var response = await _apiService.PutAsync($"rooms/{roomId}/images/{imageId}", new
        {
            Id = imageId,
            model.ImageUrl,
            model.IsMain,
            model.DisplayOrder
        });

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Oda görseli güncellendi." : response.Message ?? "Oda görseli güncellenemedi.";

        return RedirectToAction(nameof(Edit), new { id = hotelId });
    }

    [HttpPost("{hotelId}/Rooms/{roomId}/Images/{imageId}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoomImage(int hotelId, int roomId, int imageId)
    {
        var response = await _apiService.DeleteAsync($"rooms/{roomId}/images/{imageId}");

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Oda görseli silindi." : response.Message ?? "Oda görseli silinemedi.";

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

        await LoadFormDependenciesAsync(model, id);
        return model;
    }

    private async Task LoadFormDependenciesAsync(AdminHotelFormViewModel model, int hotelId = 0)
    {
        var amenityResponse = await _apiService.GetAsync<List<AmenityViewModel>>("amenities");

        model.AllAmenities = amenityResponse.Success
            ? (amenityResponse.Data ?? new())
                .Select(x => new AdminAmenityItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Icon = x.Icon,
                    IsAssigned = model.SelectedAmenityIds.Contains(x.Id)
                })
                .ToList()
            : new();

        var roomAmenityResponse = await _apiService.GetAsync<List<RoomAmenityViewModel>>("room-amenities");
        model.AllRoomAmenities = roomAmenityResponse.Success
            ? roomAmenityResponse.Data ?? new()
            : new();

        var roomTypeResponse = await _apiService.GetAsync<List<RoomTypeAdminViewModel>>("roomtypes");
        model.RoomTypes = roomTypeResponse.Success
            ? roomTypeResponse.Data ?? new List<RoomTypeAdminViewModel>()
            : new List<RoomTypeAdminViewModel>();

        // Manzara dropdown'i icin liste. Hata olursa bos dondur ki form acilsin.
        var roomViewResponse = await _apiService.GetAsync<List<RoomViewAdminViewModel>>("roomviews");
        model.RoomViews = roomViewResponse.Success
            ? roomViewResponse.Data ?? new List<RoomViewAdminViewModel>()
            : new List<RoomViewAdminViewModel>();

        if (hotelId > 0)
        {
            var roomResponse = await _apiService.GetAsync<List<AdminHotelRoomViewModel>>($"hotels/{hotelId}/rooms");
            model.Rooms = roomResponse.Success
                ? roomResponse.Data ?? new List<AdminHotelRoomViewModel>()
                : new List<AdminHotelRoomViewModel>();

            foreach(var room in model.Rooms)
            {
                var roomImagesResponse=await _apiService.GetAsync<List<RoomImageViewModel>>($"rooms/{room.Id}/images");

                room.Images= roomImagesResponse.Success
                    ? roomImagesResponse.Data ?? new()
                    : new();

                room.MainImageUrl = room.Images
                    .OrderByDescending(x => x.IsMain)
                    .ThenBy(x => x.DisplayOrder)
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault();
            }

            if (model.Images.Count == 0)
            {
                var imageResponse = await _apiService.GetAsync<List<HotelImageViewModel>>($"hotels/{hotelId}/images");
                model.Images = imageResponse.Success ? imageResponse.Data ?? new() : new();
            }
        }
    }

    private bool TryParsePrices(
        int hotelId,
        out decimal pricePerNight,
        out decimal extraGuestPricePerNight,
        out IActionResult? errorResult)
    {
        errorResult = null;

        var priceRaw = Request.Form["PricePerNight"].ToString();
        var extraRaw = Request.Form["ExtraGuestPricePerNight"].ToString();

        if (!decimal.TryParse(priceRaw.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out pricePerNight))
        {
            extraGuestPricePerNight = 0;
            TempData["ErrorMessage"] = "Gecelik fiyat formatı hatalı.";
            errorResult = RedirectToAction(nameof(Edit), new { id = hotelId });
            return false;
        }

        if (!decimal.TryParse(extraRaw.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out extraGuestPricePerNight))
        {
            TempData["ErrorMessage"] = "Ek misafir fiyatı formatı hatalı.";
            errorResult = RedirectToAction(nameof(Edit), new { id = hotelId });
            return false;
        }

        return true;
    }
}
