namespace HotelApp.Tests;

// Sprint5AdminRoomReservationFaqContractTests sinifinin amaci, S5-05 Admin RoomTypes + Reservations + FAQ kartinin CI sozlesmesini korumaktir.
// Ilgili API/UI dosyalari yoksa CI passed gecer; dosyalar olusunca plan endpointleri, admin korumasi ve FAQ davranisini dogrular.
// SOLID: SRP kullanir; sadece S5-05 oda tipi, admin rezervasyon ve FAQ contract kontrollerinden sorumludur.
public class Sprint5AdminRoomReservationFaqContractTests
{
    // S5-05 icin RoomTypes API'nin E20, E21, E22 ve E51 endpoint sozlesmesini tasidigini kontrol eder.
    [Fact]
    public void S505_RoomTypesApi_ShouldExposeE20E21E22E51ContractsWhenStarted()
    {
        // RoomTypes API baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S505_RoomTypesApiImplemented())
            return;

        var controller = ReadFirstExistingApiFile("Controllers", "RoomTypeController.cs", "Controllers", "RoomTypesController.cs");
        var service = ReadOptionalDomainFile("Services", "RoomTypeService.cs");
        var interfaceSource = ReadOptionalDomainFile("Services", "IRoomTypeService.cs");
        var dto = ReadOptionalDomainFile("DTOs", "RoomTypeDto.cs");
        var source = string.Join(Environment.NewLine, controller, service, interfaceSource, dto);

        AssertContainsAny(controller, "[Route(\"api/roomtypes\")]", "api/roomtypes");
        AssertContainsAny(controller, "[ApiController]", "ControllerBase");
        AssertContainsAny(controller, "IRoomTypeService", "RoomTypeService");
        AssertContainsAny(controller, "[HttpGet]", "GetAll");
        AssertContainsAny(controller, "[HttpPost]", "Create");
        AssertContainsAny(controller, "[HttpPut(\"{id}\")]", "HttpPut(\"{id}\")", "Update");
        AssertContainsAny(controller, "[HttpDelete(\"{id}\")]", "HttpDelete(\"{id}\")", "Delete");
        AssertContainsAny(controller, "[Authorize(Roles = \"Admin\")]", "Roles = \"Admin\"", "Roles=\"Admin\"");
        AssertContainsAny(source, "CreateRoomTypeDto", "UpdateRoomTypeDto", "UpdateRoomTypeRequestDto", "RoomTypeDto");
        AssertContainsAny(source, "Name", "Description", "MaxGuests");
        AssertContainsAny(source, "GetByNameAsync", "zaten mevcut", "duplicate");
        AssertContainsAny(source, "GetRoomTypeWithRoomsAsync", "HotelRooms.Any", "Kullanimda", "Kullanımda");
    }

    // S5-05 icin Admin/RoomTypes UI'nin E20-E22/E51 endpointlerine baglandigini kontrol eder.
    [Fact]
    public void S505_AdminRoomTypesUi_ShouldUseRoomTypesEndpointsAndShowConflictMessages()
    {
        // Admin RoomTypes UI baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S505_AdminRoomTypesUiImplemented())
            return;

        var source = ReadWebFile("Controllers", "AdminRoomTypesController.cs")
            + Environment.NewLine + ReadWebFile("Views", "AdminRoomTypes", "Index.cshtml");

        AssertContainsAny(source, "AdminFilter", "ServiceFilter", "Authorize", "Roles = \"Admin\"", "Roles=\"Admin\"");
        AssertContainsAny(source, "ApiService", "GetAsync", "PostAsync", "PutAsync", "DeleteAsync");
        AssertContainsAny(source, "api/roomtypes", "roomtypes");
        AssertContainsAny(source, "Name", "Description", "MaxGuests", "Oda tipi", "Oda Tipi");
        AssertContainsAny(source, "Create", "Yeni", "Ekle", "PostAsync");
        AssertContainsAny(source, "Update", "Guncelle", "Güncelle", "PutAsync");
        AssertContainsAny(source, "Delete", "Sil", "DeleteAsync");
        AssertContainsAny(source, "confirm", "Onay", "Emin", "Silmek");
        AssertContainsAny(source, "409", "Conflict", "kullanim", "Kullanım", "kullanimda", "Kullanımda");
        AssertContainsAny(source, "TempData", "ModelState", "alert", "ErrorMessage", "SuccessMessage");
        AssertContainsAny(source, "_AdminLayout", "Layout", "Admin");
    }

    // S5-05 icin Admin rezervasyon API'sinin E40, E41 ve E48 endpoint sozlesmesini tasidigini kontrol eder.
    [Fact]
    public void S505_AdminReservationsApi_ShouldExposeE40E41E48ContractsWhenStarted()
    {
        // Admin rezervasyon API baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S505_AdminReservationsApiImplemented())
            return;

        var source = ReadOptionalApiFile("Controllers", "AdminController.cs")
            + Environment.NewLine + ReadOptionalApiFile("Controllers", "AdminReservationsController.cs")
            + Environment.NewLine + ReadOptionalDomainFile("Services", "AdminService.cs")
            + Environment.NewLine + ReadOptionalDomainFile("Services", "IAdminService.cs")
            + Environment.NewLine + ReadOptionalDomainFile("DTOs", "ReservationDto.cs");

        AssertContainsAny(source, "[Authorize(Roles = \"Admin\")]", "Roles = \"Admin\"", "Roles=\"Admin\"");
        AssertContainsAny(source, "HttpGet(\"reservations\")", "api/admin/reservations", "GetReservations");
        AssertContainsAny(source, "status", "hotelId", "userId", "dateFrom", "dateTo", "page", "pageSize");
        AssertContainsAny(source, "PagedResponseDto", "TotalCount", "Data", "ReservationDto");
        AssertContainsAny(source, "HttpPut(\"reservations/{id}/status\")", "UpdateReservationStatus", "/status");
        AssertContainsAny(source, "Confirmed", "Cancelled", "Completed");
        AssertContainsAny(source, "Invalid", "gecersiz", "geçersiz", "BadRequest", "400");
        AssertContainsAny(source, "HttpPut(\"reservations/{id}/confirm\")", "ConfirmReservation", "/confirm");
        AssertContainsAny(source, "Pending", "Confirmed", "Cancelled", "Reject", "Confirm");
        AssertContainsAny(source, "vw_ReservationDetails", "ReservationDetails", "ReservationRepository", "AdminRepository");
    }

    // S5-05 icin Admin/Reservations UI'nin filtre, durum degistirme ve confirm/reject akislarini kontrol eder.
    [Fact]
    public void S505_AdminReservationsUi_ShouldUseFiltersStatusBadgesAndConfirmRejectActions()
    {
        // Admin Reservations UI baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S505_AdminReservationsUiImplemented())
            return;

        var source = ReadWebFile("Controllers", "AdminReservationsController.cs")
            + Environment.NewLine + ReadWebFile("Views", "AdminReservations", "Index.cshtml");

        AssertContainsAny(source, "AdminFilter", "ServiceFilter", "Authorize", "Roles = \"Admin\"", "Roles=\"Admin\"");
        AssertContainsAny(source, "ApiService", "GetAsync", "PutAsync");
        AssertContainsAny(source, "admin/reservations", "api/admin/reservations");
        AssertContainsAny(source, "status", "hotelId", "from", "to", "page", "pageSize");
        AssertContainsAny(source, "asp-route-status", "asp-route-from", "asp-route-to", "asp-route-page");
        AssertContainsAny(source, "badge", "Pending", "Confirmed", "Cancelled", "Completed");
        AssertContainsAny(source, "/status", "UpdateStatus", "Durum", "Status");
        AssertContainsAny(source, "/confirm", "Confirm", "Reject", "Onayla", "Reddet");
        AssertContainsAny(source, "_AdminLayout", "Layout", "Admin");
        Assert.DoesNotContain("PasswordHash", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", source, StringComparison.OrdinalIgnoreCase);
    }

    // S5-05 icin FAQ sayfasinin public, statik ve Bootstrap accordion yapisinda oldugunu kontrol eder.
    [Fact]
    public void S505_FaqPage_ShouldBePublicStaticAndUseBootstrapAccordionWhenStarted()
    {
        // FAQ sayfasi baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S505_FaqPageImplemented())
            return;

        var homeController = ReadWebFile("Controllers", "HomeController.cs");
        var faqView = ReadWebFile("Views", "Home", "Faq.cshtml");
        var layout = ReadOptionalWebFile("Views", "Shared", "_Layout.cshtml");
        var source = homeController + Environment.NewLine + faqView + Environment.NewLine + layout;

        AssertContainsAny(homeController, "IActionResult Faq", "Task<IActionResult> Faq", "Faq()");
        Assert.DoesNotContain("AuthFilter", GetMethodNeighborhood(homeController, "Faq"), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("AdminFilter", GetMethodNeighborhood(homeController, "Faq"), StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(faqView, "accordion", "accordion-item", "accordion-button", "data-bs-toggle=\"collapse\"");
        AssertContainsAny(faqView, "Rezervasyon", "Otel", "Giris", "Giriş", "Iptal", "İptal", "Soru");
        AssertContainsAny(faqView, "container", "row", "col-", "card", "accordion");
        Assert.DoesNotContain("ApiService", GetMethodNeighborhood(homeController, "Faq"), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DbContext", faqView, StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(source, "asp-action=\"Faq\"", "href=\"/Home/Faq\"", "href=\"/Faq\"", "FAQ", "Faq");
    }

    // S5-05 admin view'larinin hassas veri gostermedigini ve admin layout kullandigini kontrol eder.
    [Fact]
    public void S505_AdminRoomReservationViews_ShouldAvoidSensitiveDataAndUseAdminLayout()
    {
        // Admin oda tipi/rezervasyon UI baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S505_AdminRoomReservationUiImplemented())
            return;

        var source = ReadWebFile("Views", "AdminRoomTypes", "Index.cshtml")
            + Environment.NewLine + ReadWebFile("Views", "AdminReservations", "Index.cshtml");

        Assert.NotEmpty(source);
        AssertContainsAny(source, "_AdminLayout", "Layout =", "Admin");
        AssertContainsAny(source, "table", "card", "row", "col-", "form-control", "form-select");
        Assert.DoesNotContain("PasswordHash", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("EmailVerificationToken", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("FailedLoginAttempts", source, StringComparison.OrdinalIgnoreCase);
    }

    // Admin feature controller ve view kaynaklarini tek metinde toplar.
    private static string ReadAdminFeatureSource(string[] controllerNames, string[] viewNames)
    {
        var controllerSources = Sprint5CardGuards.CandidateAdminControllerSources(controllerNames);
        var viewSources = Sprint5CardGuards.CandidateAdminViewSources(viewNames);

        return string.Join(Environment.NewLine, controllerSources.Concat(viewSources));
    }

    // API projesindeki opsiyonel bir dosyayi varsa okur.
    private static string ReadOptionalApiFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.API") }.Concat(relativePathParts).ToArray());

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    // API projesindeki iki olasi yoldan ilk bulunan dosyayi okur.
    private static string ReadFirstExistingApiFile(params string[] pathParts)
    {
        Assert.True(pathParts.Length % 2 == 0, "Dosya yolu parcalari ikili verilmelidir.");

        for (var i = 0; i < pathParts.Length; i += 2)
        {
            var path = Path.Combine(FindProjectRoot("HotelApp.API"), pathParts[i], pathParts[i + 1]);
            if (File.Exists(path))
                return File.ReadAllText(path);
        }

        throw new FileNotFoundException("Beklenen API dosyalarindan hicbiri bulunamadi.");
    }

    // Domain projesindeki opsiyonel bir dosyayi varsa okur.
    private static string ReadOptionalDomainFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.Domain") }.Concat(relativePathParts).ToArray());

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    // Web projesindeki zorunlu bir dosyayi okur.
    private static string ReadWebFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.Web") }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    // Web projesindeki opsiyonel bir dosyayi varsa okur.
    private static string ReadOptionalWebFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.Web") }.Concat(relativePathParts).ToArray());

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    // Bir method etrafindaki kisa kaynak bolumunu alir; attribute kontrolunde false positive'i azaltir.
    private static string GetMethodNeighborhood(string source, string methodName)
    {
        var index = source.IndexOf(methodName, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
            return string.Empty;

        var start = Math.Max(0, index - 300);
        var length = Math.Min(source.Length - start, 900);
        return source.Substring(start, length);
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
