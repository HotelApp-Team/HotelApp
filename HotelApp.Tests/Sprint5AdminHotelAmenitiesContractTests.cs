namespace HotelApp.Tests;

// Sprint5AdminHotelAmenitiesContractTests sinifinin amaci, S5-03 Admin Hotels + HotelEdit + Amenities UI kartinin CI sozlesmesini korumaktir.
// Admin otel/olanak UI dosyalari yokken CI passed gecer; dosyalar olusunca plan endpointleri, admin korumasi ve view davranislarini dogrular.
// SOLID: SRP kullanir; sadece S5-03 admin otel, gorsel ve olanak yonetimi contract kontrollerinden sorumludur.
public class Sprint5AdminHotelAmenitiesContractTests
{
    // S5-03 icin baslayan admin controller'larin admin yetkisi ve ApiService bagimliligi kullandigini kontrol eder.
    [Fact]
    public void S503_AdminHotelAmenityControllers_ShouldRequireAdminAccessWhenStarted()
    {
        // S5-03 UI baslamadiysa CI passed gecer; admin controller dosyalari eklenince contract aktif olur.
        if (!Sprint5CardGuards.S503_AdminHotelAmenitiesUiImplemented())
            return;

        var sources = ReadStartedS503ControllerSources();

        Assert.NotEmpty(sources);
        foreach (var source in sources)
        {
            AssertContainsAny(source, "AdminFilter", "ServiceFilter", "Authorize", "Roles = \"Admin\"", "Roles=\"Admin\"");
            AssertContainsAny(source, "ApiService", "GetAsync", "PostAsync", "PutAsync", "DeleteAsync");
        }
    }

    // S5-03 icin Admin/Hotels ekraninin E04, E08 ve E10 endpointlerine baglandigini kontrol eder.
    [Fact]
    public void S503_HotelMgmtUi_ShouldCallE04E08E10AndRenderListCreateDeleteFlows()
    {
        // Otel yonetimi ekrani baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S503_HotelMgmtUiImplemented())
            return;

        var source = ReadAdminFeatureSource(
            controllerNames: new[] { "HotelMgmtController.cs", "AdminHotelsController.cs" },
            viewNames: new[] { "Hotels.cshtml", Path.Combine("Hotels", "Index.cshtml"), Path.Combine("HotelMgmt", "Index.cshtml") });

        AssertContainsAny(source, "api/hotels", "\"hotels", "'hotels");
        AssertContainsAny(source, "GetAsync", "GetTAsync");
        AssertContainsAny(source, "PostAsync", "Create", "Yeni Otel", "Otel Ekle");
        AssertContainsAny(source, "DeleteAsync", "Delete", "Sil");
        AssertContainsAny(source, "search", "Search", "arama", "Arama");
        AssertContainsAny(source, "city", "City", "Sehir");
        AssertContainsAny(source, "star", "StarRating", "Yildiz");
        AssertContainsAny(source, "sort", "Sort", "Siralama");
        AssertContainsAny(source, "page", "Page", "CurrentPage");
        AssertContainsAny(source, "pageSize", "PageSize");
        AssertContainsAny(source, "IsActive", "Aktif", "soft", "pasif");
        AssertContainsAny(source, "TempData", "ModelState", "alert", "ErrorMessage", "SuccessMessage");
        AssertContainsAny(source, "_AdminLayout", "Layout", "Admin");
        Assert.DoesNotContain("PasswordHash", source, StringComparison.OrdinalIgnoreCase);
    }

    // S5-03 icin HotelEdit ekraninin E05, E09 ve E17-E19 gorsel endpointlerine baglandigini kontrol eder.
    [Fact]
    public void S503_HotelEditUi_ShouldCallE05E09AndImageEndpointsWithoutInventedCoverRoute()
    {
        // Otel duzenleme ekrani baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S503_HotelEditUiImplemented())
            return;

        var source = ReadAdminFeatureSource(
            controllerNames: new[] { "HotelMgmtController.cs", "AdminHotelsController.cs" },
            viewNames: new[] { "HotelEdit.cshtml", Path.Combine("Hotels", "Edit.cshtml"), Path.Combine("HotelMgmt", "Edit.cshtml") });

        AssertContainsAny(source, "GetAsync", "GetTAsync");
        AssertContainsAny(source, "PutAsync", "Update", "Guncelle");
        AssertContainsAny(source, "hotels/{id}", "hotels/", "api/hotels");
        AssertContainsAny(source, "Name", "Address", "City", "StarRating", "Description");
        AssertContainsAny(source, "images", "HotelImage", "Gorsel", "ImageUrl");
        AssertContainsAny(source, "PostAsync", "Upload", "Ekle");
        AssertContainsAny(source, "hotels/{hotelId}/images", "/images", "images/");
        AssertContainsAny(source, "images/{imageId}", "imageId", "PutAsync", "DeleteAsync");
        AssertContainsAny(source, "IsMain", "Ana Gorsel", "MainImage");
        Assert.False(
            source.Contains("/cover", StringComparison.OrdinalIgnoreCase)
                || source.Contains("cover/", StringComparison.OrdinalIgnoreCase),
            "S5-03 planina gore ana gorsel secimi E18 ile yapilmali; ayri cover route uydurulmamali.");
        AssertContainsAny(source, "TempData", "ModelState", "alert", "ErrorMessage", "SuccessMessage");
    }

    // S5-03 icin HotelEdit ekraninin E34-E35 otel-olanak atama/kaldirma akislarini icerdigini kontrol eder.
    [Fact]
    public void S503_HotelEditUi_ShouldCallE34E35ForHotelAmenityAssignment()
    {
        // Otel duzenleme ekrani baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S503_HotelEditUiImplemented())
            return;

        var source = ReadAdminFeatureSource(
            controllerNames: new[] { "HotelMgmtController.cs", "AdminHotelsController.cs" },
            viewNames: new[] { "HotelEdit.cshtml", Path.Combine("Hotels", "Edit.cshtml"), Path.Combine("HotelMgmt", "Edit.cshtml") });

        AssertContainsAny(source, "amenities", "Amenity", "Olanak");
        AssertContainsAny(source, "api/amenities", "GetAsync", "GetTAsync");
        AssertContainsAny(source, "hotels/{hotelId}/amenities", "/amenities", "Assign", "Ata");
        AssertContainsAny(source, "PostAsync", "AssignAmenity", "AddAmenity", "OlanakAta");
        AssertContainsAny(source, "amenities/{amenityId}", "amenityId", "RemoveAmenity", "DeleteAsync", "Kaldir");
        AssertContainsAny(source, "checkbox", "select", "asp-for", "asp-items", "form-check");
        AssertContainsAny(source, "404", "409", "Conflict", "TempData", "ModelState", "alert");
    }

    // S5-03 icin Admin/Amenities ekraninin E32, E33, E49 ve E50 endpointlerine baglandigini kontrol eder.
    [Fact]
    public void S503_AmenityUi_ShouldCallE32E33E49E50AndRenderCrudFlow()
    {
        // Olanak yonetimi ekrani baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S503_AmenityUiImplemented())
            return;

        // Olanak yonetimi tek sayfada toplandi: AmenityMgmtController/AmenityMgmt.cshtml.
        // Eski Amenity/RoomAmenity controller'lar CRUD endpoint olarak korundu, eski view'lar silindi.
        var source = ReadAdminFeatureSource(
            controllerNames: new[] { "AmenityController.cs", "AmenitiesController.cs", "AdminAmenitiesController.cs", "AmenityMgmtController.cs" },
            viewNames: new[] { "Amenities.cshtml", Path.Combine("Amenities", "Index.cshtml"), Path.Combine("Amenity", "Index.cshtml"), "AmenityMgmt.cshtml" });

        AssertContainsAny(source, "api/amenities", "\"amenities", "'amenities");
        AssertContainsAny(source, "GetAsync", "GetTAsync");
        AssertContainsAny(source, "PostAsync", "Create", "Yeni Olanak", "Olanak Ekle");
        AssertContainsAny(source, "PutAsync", "Update", "Guncelle");
        AssertContainsAny(source, "DeleteAsync", "Delete", "Sil");
        AssertContainsAny(source, "Name", "AmenityName", "Olanak");
        AssertContainsAny(source, "Icon", "ikon", "Ikon");
        AssertContainsAny(source, "409", "Conflict", "kullanim", "TempData", "ModelState", "alert");
        AssertContainsAny(source, "confirm", "Onay", "Silmek", "Emin");
        AssertContainsAny(source, "_AdminLayout", "Layout", "Admin");
    }

    // S5-03 view'larinin hassas veri gostermedigini ve admin layout/responsive izlerini korudugunu kontrol eder.
    [Fact]
    public void S503_AdminHotelAmenityViews_ShouldAvoidSensitiveDataAndUseAdminResponsiveMarkup()
    {
        // S5-03 UI baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S503_AdminHotelAmenitiesUiImplemented())
            return;

        var viewSource = string.Join(Environment.NewLine, ReadStartedS503ViewSources());

        Assert.NotEmpty(viewSource);
        AssertContainsAny(viewSource, "_AdminLayout", "Layout =", "Admin");
        AssertContainsAny(viewSource, "table", "card", "row", "col-", "form-control");
        Assert.DoesNotContain("PasswordHash", viewSource, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", viewSource, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("EmailVerificationToken", viewSource, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Token", viewSource, StringComparison.OrdinalIgnoreCase);
    }

    // S5-03 UI basladiysa API controller kaynaklarinda plan endpointlerinin mevcut oldugunu kontrol eder.
    [Fact]
    public void S503_ApiSources_ShouldExposePlannedHotelImageAmenityRoutesWhenUiStarts()
    {
        // S5-03 UI baslamadiysa CI passed gecer; UI baslayinca backend plan uyumu da gorunur olur.
        if (!Sprint5CardGuards.S503_AdminHotelAmenitiesUiImplemented())
            return;

        var hotelsApi = ReadProjectFile("HotelApp.API", "Controllers", "HotelsController.cs")
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.API", "Controllers", "HotelImagesController.cs");
        var amenitiesApi = ReadOptionalProjectFile("HotelApp.API", "Controllers", "AmenitiesController.cs")
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.API", "Controllers", "HotelAmenitiesController.cs");

        AssertContainsAny(hotelsApi, "HttpGet", "GetAll", "GetById");
        AssertContainsAny(hotelsApi, "HttpPost", "Create");
        AssertContainsAny(hotelsApi, "HttpPut", "Update");
        AssertContainsAny(hotelsApi, "HttpDelete", "Delete");
        AssertContainsAny(hotelsApi, "images", "HotelImage");
        AssertContainsAny(hotelsApi, "hotelId", "imageId");

        AssertContainsAny(amenitiesApi, "HttpGet", "GetAll", "amenities");
        AssertContainsAny(amenitiesApi, "HttpPost", "Create", "assign");
        AssertContainsAny(amenitiesApi, "HttpPut", "Update");
        AssertContainsAny(amenitiesApi, "HttpDelete", "Delete", "remove");
        AssertContainsAny(amenitiesApi, "hotelId", "amenityId", "Amenity");
    }

    // Baslamis S5-03 controller kaynaklarini bir araya getirir.
    private static List<string> ReadStartedS503ControllerSources()
    {
        var sources = new List<string>();

        // Hangi parca basladiysa sadece onun controller dosyalarini kapsama aliriz.
        if (Sprint5CardGuards.S503_HotelMgmtUiImplemented() || Sprint5CardGuards.S503_HotelEditUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminControllerSources("HotelMgmtController.cs", "AdminHotelsController.cs"));

        if (Sprint5CardGuards.S503_AmenityUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminControllerSources("AmenityController.cs", "AmenitiesController.cs", "AdminAmenitiesController.cs"));

        return sources.ToList();
    }

    // Baslamis S5-03 view kaynaklarini bir araya getirir.
    private static List<string> ReadStartedS503ViewSources()
    {
        var sources = new List<string>();

        // Hangi ekran basladiysa sadece onun view dosyalarini okuruz.
        if (Sprint5CardGuards.S503_HotelMgmtUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminViewSources("Hotels.cshtml", Path.Combine("Hotels", "Index.cshtml"), Path.Combine("HotelMgmt", "Index.cshtml")));

        if (Sprint5CardGuards.S503_HotelEditUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminViewSources("HotelEdit.cshtml", Path.Combine("Hotels", "Edit.cshtml"), Path.Combine("HotelMgmt", "Edit.cshtml")));

        if (Sprint5CardGuards.S503_AmenityUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminViewSources("Amenities.cshtml", Path.Combine("Amenities", "Index.cshtml"), Path.Combine("Amenity", "Index.cshtml"), "AmenityMgmt.cshtml"));

        return sources.ToList();
    }

    // Bir admin ozelligine ait controller ve view kaynaklarini tek metinde toplar.
    private static string ReadAdminFeatureSource(string[] controllerNames, string[] viewNames)
    {
        var controllerSources = Sprint5CardGuards.CandidateAdminControllerSources(controllerNames);
        var viewSources = Sprint5CardGuards.CandidateAdminViewSources(viewNames);

        return string.Join(Environment.NewLine, controllerSources.Concat(viewSources));
    }

    // Projedeki zorunlu bir dosyayi okur.
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
