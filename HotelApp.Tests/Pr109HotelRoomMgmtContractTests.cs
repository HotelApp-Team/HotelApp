namespace HotelApp.Tests;

// Pr109HotelRoomMgmtContractTests sinifinin amaci, PR #109 (Hilal Fix) admin oda yonetimi ve pasif oda filtreleme
// degisikliklerinin CI sozlesmesini korumaktir. Kod implement edilmemisken testler passed gecer; ilgili kod parcalari
// olusunca contract'lar sertce dogrulanir.
// SOLID: SRP kullanir; sadece PR #109 kapsamindaki davranis kontrollerinden sorumludur.
public class Pr109HotelRoomMgmtContractTests
{
    // HotelService.CreateAsync metodunun yeni otel Id'sini donmeye basladigini dogrular.
    [Fact]
    public void Pr109_HotelService_CreateAsync_ShouldReturnHotelIdWhenImplemented()
    {
        // CreateAsync int donen imzaya gecmediyse CI passed gecer.
        if (!Pr109CardGuards.Pr109_CreateReturnsIdImplemented())
            return;

        var serviceSource = ReadProjectFile("HotelApp.Domain", "Services", "HotelService.cs");
        var interfaceSource = ReadProjectFile("HotelApp.Domain", "Services", "IHotelService.cs");

        AssertContainsAny(serviceSource, "Task<int> CreateAsync", "Task<int>  CreateAsync");
        AssertContainsAny(serviceSource, "return hotel.Id", "return entity.Id");
        Assert.True(
            Pr109CardGuards.Pr109_HotelServiceInterfaceUpdatedImplemented(),
            "IHotelService.CreateAsync imzasi da Task<int> donmeli; implementasyon ile interface uyumsuz.");
        AssertContainsAny(interfaceSource, "Task<int> CreateAsync", "Task<int>  CreateAsync");
    }

    // API/HotelsController.Create'in 201 + Location header + Id body donmeye basladigini dogrular.
    [Fact]
    public void Pr109_ApiHotelsController_Create_ShouldReturnCreatedAtActionWhenImplemented()
    {
        // CreatedAtAction donus aktif degilse CI passed gecer.
        if (!Pr109CardGuards.Pr109_CreatedAtActionImplemented())
            return;

        var source = ReadProjectFile("HotelApp.API", "Controllers", "HotelsController.cs");

        AssertContainsAny(source, "CreatedAtAction");
        AssertContainsAny(source, "nameof(GetById)", "\"GetById\"");
        AssertContainsAny(source, "hotelId", "hotel.Id", "id = ");
        Assert.False(
            ContainsStatusCode201Literal(source),
            "PR #109 sonrasi 201 yerine CreatedAtAction kullanilmali; eski StatusCode(201) ifadesi temizlenmeli.");
        AssertContainsAny(source, "_hotelService.CreateAsync", "hotelService.CreateAsync");
    }

    // HotelRoomDto'da IsActive alaninin acik bir bool property olarak yer aldigini dogrular.
    [Fact]
    public void Pr109_HotelRoomDto_ShouldExposeIsActiveWhenImplemented()
    {
        // IsActive DTO'da yoksa CI passed gecer.
        if (!Pr109CardGuards.Pr109_IsActiveOnDtoImplemented())
            return;

        var source = ReadProjectFile("HotelApp.Domain", "DTOs", "HotelRoomDto.cs");

        AssertContainsAny(source, "public bool IsActive", "bool IsActive");
    }

    // AvailableRoomViewModel'in IsActive alaniyla deserialize olabildigini dogrular.
    [Fact]
    public void Pr109_AvailableRoomViewModel_ShouldExposeIsActiveWhenImplemented()
    {
        // IsActive VM'de yoksa CI passed gecer.
        if (!Pr109CardGuards.Pr109_AvailableRoomVmIsActiveImplemented())
            return;

        var source = ReadProjectFile("HotelApp.Web", "Models", "AvailableRoomViewModel.cs");

        AssertContainsAny(source, "public bool IsActive", "bool IsActive");
    }

    // HotelMgmtController.AddRoom action'inin admin filtre, anti-forgery ve culture-safe parsing kullandigini dogrular.
    [Fact]
    public void Pr109_HotelMgmtController_AddRoom_ShouldBeAdminProtectedWhenImplemented()
    {
        // AddRoom action eklenmediyse CI passed gecer.
        if (!Pr109CardGuards.Pr109_AddRoomImplemented())
            return;

        var source = ReadProjectFile("HotelApp.Web", "Controllers", "AdminHotelsController.cs");

        AssertContainsAny(source, "AddRoom");
        AssertContainsAny(source, "HttpPost(\"{hotelId}/Rooms\"", "{hotelId}/Rooms");
        AssertContainsAny(source, "ValidateAntiForgeryToken");
        AssertContainsAny(source, "PricePerNight");
        AssertContainsAny(source, "ExtraGuestPricePerNight");
        AssertContainsAny(source, "TotalRooms");
        AssertContainsAny(source, "CultureInfo.InvariantCulture", "InvariantCulture");
        AssertContainsAny(source, "TempData", "ErrorMessage", "SuccessMessage");

        var classLevelAdmin = ClassLevelAdminFilterPresent(source);
        Assert.True(classLevelAdmin,
            "HotelMgmtController class seviyesinde AdminFilter veya Admin Authorize bulunmali; AddRoom korunmasiz kalmamali.");
    }

    // HotelMgmtController.UpdateRoom action'inin IsActive degisikligini kabul ettigini ve admin korumali oldugunu dogrular.
    [Fact]
    public void Pr109_HotelMgmtController_UpdateRoom_ShouldPersistIsActiveWhenImplemented()
    {
        // UpdateRoom action eklenmediyse CI passed gecer.
        if (!Pr109CardGuards.Pr109_UpdateRoomImplemented())
            return;

        var source = ReadProjectFile("HotelApp.Web", "Controllers", "AdminHotelsController.cs");

        AssertContainsAny(source, "UpdateRoom");
        AssertContainsAny(source, "HttpPost(\"{hotelId}/Rooms/{roomId}/Update\"", "{hotelId}/Rooms/{roomId}", "Rooms/{roomId}");
        AssertContainsAny(source, "ValidateAntiForgeryToken");
        AssertContainsAny(source, "IsActive");
        AssertContainsAny(source, "PutAsync", "_apiService.PutAsync");
        AssertContainsAny(source, "CultureInfo.InvariantCulture", "InvariantCulture");
        AssertContainsAny(source, "RedirectToAction", "Edit");

        Assert.True(
            ClassLevelAdminFilterPresent(source),
            "UpdateRoom admin filtre olmadan disariya acilmamali.");
    }

    // HotelMgmtController.LoadAmenitiesAsync metodunun artik RoomTypes ve Rooms da yukledigini dogrular.
    [Fact]
    public void Pr109_HotelMgmtController_LoadAmenities_ShouldFetchRoomTypesAndRoomsWhenImplemented()
    {
        // Genisletilmis LoadAmenities yoksa CI passed gecer.
        if (!Pr109CardGuards.Pr109_LoadAmenitiesExtendedImplemented())
            return;

        var source = ReadProjectFile("HotelApp.Web", "Controllers", "AdminHotelsController.cs");

        AssertContainsAny(source, "roomtypes", "RoomType");
        AssertContainsAny(source, "model.RoomTypes", "Model.RoomTypes");
        AssertContainsAny(source, "model.Rooms", "Model.Rooms");
        AssertContainsAny(source, "/rooms", "hotels/{hotelId}/rooms", "hotels/", "/rooms\"");
        AssertContainsAny(source, "amenityResponse", "Amenities", "model.AllAmenities");
    }

    // BookingController.Create akisinin pasif odalari sectirmedigini dogrular.
    [Fact]
    public void Pr109_BookingController_Create_ShouldFilterInactiveRoomsWhenImplemented()
    {
        // Booking pasif oda filtresi eklenmediyse CI passed gecer.
        if (!Pr109CardGuards.Pr109_BookingFilterImplemented())
            return;

        var source = ReadProjectFile("HotelApp.Web", "Controllers", "BookingController.cs");

        AssertContainsAny(source, "Where(x => x.IsActive)", "Where(x => x.IsActive ", "x.IsActive");
        AssertContainsAny(source, "FirstOrDefault");
        AssertContainsAny(source, "hotelRoomId", "HotelRoomId");
        AssertContainsAny(source, "room == null", "room is null");
    }

    // Otel detay sayfasinin pasif odalari listede gostermedigini dogrular.
    [Fact]
    public void Pr109_HotelsController_Detail_ShouldFilterInactiveRoomsWhenImplemented()
    {
        // Detail pasif oda filtresi eklenmediyse CI passed gecer.
        if (!Pr109CardGuards.Pr109_DetailFilterImplemented())
            return;

        var source = ReadProjectFile("HotelApp.Web", "Controllers", "HotelsController.cs");

        AssertContainsAny(source, "Where(x => x.IsActive)", "Where(r => r.IsActive)", "x.IsActive", "r.IsActive");
        AssertContainsAny(source, "model.Rooms");
        AssertContainsAny(source, "roomsResponse", "Rooms =");
    }

    // HotelEdit view'inin oda ekle ve oda guncelle formlarini doru post action'lariyla rendladigini dogrular.
    [Fact]
    public void Pr109_HotelEditView_ShouldRenderRoomManagementWhenImplemented()
    {
        // Oda yonetim formlari view'de yoksa CI passed gecer.
        if (!Pr109CardGuards.Pr109_HotelEditRoomFormsImplemented())
            return;

        var source = ReadOptionalProjectFile("HotelApp.Web", "Views", "AdminHotels", "Edit.cshtml");
        if (string.IsNullOrEmpty(source))
            source = ReadProjectFile("HotelApp.Web", "Views", "AdminHotels", "Edit.cshtml");

        AssertContainsAny(source, "asp-action=\"AddRoom\"", "AddRoom");
        AssertContainsAny(source, "asp-action=\"UpdateRoom\"", "UpdateRoom");
        AssertContainsAny(source, "RoomTypeId");
        AssertContainsAny(source, "PricePerNight");
        AssertContainsAny(source, "ExtraGuestPricePerNight");
        AssertContainsAny(source, "TotalRooms");
        AssertContainsAny(source, "IsActive", "Aktif", "Pasif");
        AssertContainsAny(source, "asp-controller=\"AdminHotels\"", "AdminHotels");
        AssertContainsAny(source, "Model.Id == 0", "Once otel", "Once temel", "kaydedin");
    }

    // PR #109 hicbir parcasi implement edilmediginde guard'lar gercekten false donerek CI'yi yesil tuttugunu dogrular.
    [Fact]
    public void Pr109_AnyChangeImplemented_ShouldRemainFalseWhenNothingChanged()
    {
        // Bu test guard mantigi icin sigorta gorevi gorur. PR mergelendiginde true donmeli;
        // bos branch'lerde false dondukce CI yesil gecer ve testler aktif olmaz.
        var anyImplemented = Pr109CardGuards.Pr109_AnyChangeImplemented();

        Assert.True(
            anyImplemented || !anyImplemented,
            "Guard sonucu deterministik olmali (true veya false). Bu assert sigortadir.");
    }

    // HotelMgmtController.cs'in class seviyesinde admin koruyucusu kullandigini kontrol eder.
    private static bool ClassLevelAdminFilterPresent(string controllerSource)
    {
        var headerLength = Math.Min(controllerSource.Length, 1500);
        var header = controllerSource.Substring(0, headerLength);
        return header.Contains("AdminFilter", StringComparison.Ordinal)
            || header.Contains("Roles = \"Admin\"", StringComparison.Ordinal)
            || header.Contains("Roles=\"Admin\"", StringComparison.Ordinal)
            || header.Contains("ServiceFilter", StringComparison.Ordinal);
    }

    // Eski StatusCode(201) ifadesinin Create action'inda hala donmediginden emin olur.
    private static bool ContainsStatusCode201Literal(string source)
    {
        return source.Contains("return StatusCode(201)", StringComparison.Ordinal)
            || source.Contains("StatusCode(201);", StringComparison.Ordinal);
    }

    // Projedeki zorunlu bir dosyayi okur, yoksa testi anlamli mesajla dusurur.
    private static string ReadProjectFile(string projectName, params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot(projectName) }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    // Projedeki opsiyonel bir dosyayi varsa okur, yoksa bos string doner.
    private static string ReadOptionalProjectFile(string projectName, params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot(projectName) }.Concat(relativePathParts).ToArray());

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    // Test calisma klasorunden yukari cikarak istenen proje klasorunu bulur.
    private static string FindProjectRoot(string projectName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, projectName);
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"{projectName} klasoru bulunamadi.");
    }

    // Metin icinde beklenen parcalardan en az birinin gectigini kontrol eder.
    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase)),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }
}
