namespace HotelApp.Tests;

// Pr109CardGuards sinifinin amaci, PR #109 (Hilal Fix) icin sartli CI testlerinin ne zaman devreye girecegini belirlemektir.
// Henuz implementasyon yokken testler pass gecer; ilgili kod parcalari olusunca contract testleri calisir.
// SOLID: SRP kullanir; sadece PR #109 davranis sinyallerini kontrol eder.
internal static class Pr109CardGuards
{
    // HotelService.CreateAsync metodunun yeni Id donen imzaya gectigini kontrol eder.
    public static bool Pr109_CreateReturnsIdImplemented()
    {
        var source = ReadDomainFile("Services", "HotelService.cs");
        return ContainsAll(source, "Task<int>", "CreateAsync")
            && ContainsAny(source, "return hotel.Id", "return entity.Id");
    }

    // IHotelService.CreateAsync imzasinin da int donmeye guncellendigini kontrol eder.
    public static bool Pr109_HotelServiceInterfaceUpdatedImplemented()
    {
        var source = ReadDomainFile("Services", "IHotelService.cs");
        return ContainsAll(source, "Task<int>", "CreateAsync");
    }

    // API/HotelsController.Create'in 201 yerine CreatedAtAction donmeye baslayip baslamadigini kontrol eder.
    public static bool Pr109_CreatedAtActionImplemented()
    {
        var source = ReadProjectFile("HotelApp.API", "Controllers", "HotelsController.cs");
        return source.Contains("CreatedAtAction", StringComparison.Ordinal)
            && ContainsAny(source, "nameof(GetById)", "\"GetById\"");
    }

    // HotelRoomDto'ya IsActive alaninin eklendigini kontrol eder.
    public static bool Pr109_IsActiveOnDtoImplemented()
    {
        var source = ReadDomainFile("DTOs", "HotelRoomDto.cs");
        return source.Contains("IsActive", StringComparison.Ordinal)
            && source.Contains("bool", StringComparison.Ordinal);
    }

    // AvailableRoomViewModel'a IsActive alaninin eklendigini kontrol eder.
    public static bool Pr109_AvailableRoomVmIsActiveImplemented()
    {
        var source = ReadProjectFile("HotelApp.Web", "Models", "AvailableRoomViewModel.cs");
        return source.Contains("IsActive", StringComparison.Ordinal)
            && source.Contains("bool", StringComparison.Ordinal);
    }

    // HotelMgmtController icinde yeni AddRoom action'inin eklendigini kontrol eder.
    public static bool Pr109_AddRoomImplemented()
    {
        var source = ReadProjectFile("HotelApp.Web", "Controllers", "HotelMgmtController.cs");
        return source.Contains("AddRoom", StringComparison.Ordinal)
            && ContainsAny(source, "{hotelId}/Rooms", "/Rooms");
    }

    // HotelMgmtController icinde yeni UpdateRoom action'inin eklendigini kontrol eder.
    public static bool Pr109_UpdateRoomImplemented()
    {
        var source = ReadProjectFile("HotelApp.Web", "Controllers", "HotelMgmtController.cs");
        return source.Contains("UpdateRoom", StringComparison.Ordinal)
            && ContainsAny(source, "/Rooms/{roomId}/Update", "Rooms/{roomId}");
    }

    // HotelMgmtController.LoadAmenitiesAsync'in RoomTypes ve Rooms da yuklemeye basladigini kontrol eder.
    public static bool Pr109_LoadAmenitiesExtendedImplemented()
    {
        var source = ReadProjectFile("HotelApp.Web", "Controllers", "HotelMgmtController.cs");
        return ContainsAny(source, "roomTypeResponse", "RoomTypes =", "model.RoomTypes")
            && ContainsAny(source, "roomResponse", "model.Rooms", "Model.Rooms");
    }

    // BookingController.Create'in pasif odalari filtrelemeye basladigini kontrol eder.
    public static bool Pr109_BookingFilterImplemented()
    {
        var source = ReadProjectFile("HotelApp.Web", "Controllers", "BookingController.cs");
        return ContainsAny(source, "x.IsActive", ".IsActive", "Where(x =>");
    }

    // Web/HotelsController.Detail'in pasif odalari filtrelemeye basladigini kontrol eder.
    public static bool Pr109_DetailFilterImplemented()
    {
        var source = ReadProjectFile("HotelApp.Web", "Controllers", "HotelsController.cs");
        return source.Contains("IsActive", StringComparison.Ordinal)
            && ContainsAny(source, "Where(x =>", "Where(r =>", "Where(room =>");
    }

    // HotelEdit.cshtml view'inde oda ekleme/guncelleme formlarinin yer aldigini kontrol eder.
    public static bool Pr109_HotelEditRoomFormsImplemented()
    {
        var source = ReadOptionalProjectFile("HotelApp.Web", "Views", "Admin", "HotelEdit.cshtml");
        if (string.IsNullOrEmpty(source))
            source = ReadOptionalProjectFile("HotelApp.Web", "Views", "HotelMgmt", "Edit.cshtml");

        return ContainsAll(source, "AddRoom", "UpdateRoom");
    }

    // PR #109 kapsaminin herhangi bir parcasinin implement edilip edilmedigini topluca kontrol eder.
    public static bool Pr109_AnyChangeImplemented()
    {
        return Pr109_CreateReturnsIdImplemented()
            || Pr109_CreatedAtActionImplemented()
            || Pr109_IsActiveOnDtoImplemented()
            || Pr109_AvailableRoomVmIsActiveImplemented()
            || Pr109_AddRoomImplemented()
            || Pr109_UpdateRoomImplemented()
            || Pr109_LoadAmenitiesExtendedImplemented()
            || Pr109_BookingFilterImplemented()
            || Pr109_DetailFilterImplemented()
            || Pr109_HotelEditRoomFormsImplemented();
    }

    // Domain projesindeki bir dosyanin metnini doner; yoksa bos string doner.
    private static string ReadDomainFile(params string[] relativePathParts)
    {
        return ReadOptionalProjectFile("HotelApp.Domain", relativePathParts);
    }

    // Belirtilen projedeki dosyayi okur; yoksa bos string doner ki guard false donsun.
    private static string ReadProjectFile(string projectName, params string[] relativePathParts)
    {
        return ReadOptionalProjectFile(projectName, relativePathParts);
    }

    // Belirtilen projedeki opsiyonel dosyayi varsa okur, yoksa bos string doner.
    private static string ReadOptionalProjectFile(string projectName, params string[] relativePathParts)
    {
        var projectRoot = FindProjectRoot(projectName);
        if (projectRoot is null)
            return string.Empty;

        var fullPath = Path.Combine(new[] { projectRoot }.Concat(relativePathParts).ToArray());
        return File.Exists(fullPath) ? File.ReadAllText(fullPath) : string.Empty;
    }

    // Test calisma klasorunden yukari cikarak istenen proje klasorunu bulur, yoksa null doner.
    private static string? FindProjectRoot(string projectName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, projectName);
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        return null;
    }

    // Metin icinde beklenen parcalarin tamaminin gectigini kontrol eder.
    private static bool ContainsAll(string text, params string[] expectedParts)
    {
        return expectedParts.All(part => text.Contains(part, StringComparison.OrdinalIgnoreCase));
    }

    // Metin icinde beklenen parcalardan en az birinin gectigini kontrol eder.
    private static bool ContainsAny(string text, params string[] expectedParts)
    {
        return expectedParts.Any(part => text.Contains(part, StringComparison.OrdinalIgnoreCase));
    }
}
