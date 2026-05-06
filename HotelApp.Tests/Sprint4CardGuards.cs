namespace HotelApp.Tests;

internal static class Sprint4CardGuards
{
    // S4-01 - Admin API karti. E36-E39 endpoint rotalarindan biri yoksa
    // testler pass gecer; endpoint kodu yazilmaya baslayinca contract testleri devreye girer.
    public static bool S401_AdminApiImplemented()
    {
        return S401_E36DashboardImplemented()
            || S401_E37UsersImplemented()
            || S401_E38StatusImplemented()
            || S401_E39RoleImplemented();
    }

    public static bool S401_E36DashboardImplemented()
    {
        return AdminControllerSourceContains("dashboard");
    }

    public static bool S401_E37UsersImplemented()
    {
        var source = ReadAdminControllerSource();

        return source.Contains("HttpGet(\"users", StringComparison.OrdinalIgnoreCase)
            || source.Contains("HttpGet(\"/users", StringComparison.OrdinalIgnoreCase)
            || source.Contains("GetUsers", StringComparison.OrdinalIgnoreCase);
    }

    public static bool S401_E38StatusImplemented()
    {
        return AdminControllerSourceContains("users/{id}/status");
    }

    public static bool S401_E39RoleImplemented()
    {
        var source = ReadAdminControllerSource();

        return source.Contains("users/{id}/roles", StringComparison.OrdinalIgnoreCase)
            || source.Contains("users/{id}/role", StringComparison.OrdinalIgnoreCase);
    }

    // S4-02 - Profil + Sifre Degistirme UI karti.
    // ProfileController/Profile view veya profile linki gibi kart-spesifik kod yoksa testler pass gecer.
    // Bu karttan herhangi bir iz gorunurse S4-02 contract testleri devreye girer.
    public static bool S402_ProfileUiImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var profileControllerPath = Path.Combine(webRoot, "Controllers", "ProfileController.cs");
        var profileViewPath = Path.Combine(webRoot, "Views", "Profile", "Index.cshtml");
        var profileViewFolder = Path.Combine(webRoot, "Views", "Profile");
        var modelsRoot = Path.Combine(webRoot, "Models");
        var loginPartialPath = Path.Combine(webRoot, "Views", "Shared", "_LoginPartial.cshtml");

        var profileModelsExist = Directory.Exists(modelsRoot)
            && Directory.GetFiles(modelsRoot, "*Profile*.cs", SearchOption.TopDirectoryOnly).Any();

        var loginPartialHasProfileLink = File.Exists(loginPartialPath)
            && File.ReadAllText(loginPartialPath).Contains("asp-controller=\"Profile\"", StringComparison.OrdinalIgnoreCase);

        return File.Exists(profileControllerPath)
            || File.Exists(profileViewPath)
            || Directory.Exists(profileViewFolder)
            || profileModelsExist
            || loginPartialHasProfileLink;
    }

    // S4-03 - About Sayfasi karti.
    // HomeController.About, Home/About view veya navbar About linki yoksa testler pass gecer.
    // Karttan herhangi bir iz gorunurse S4-03 contract testleri devreye girer.
    public static bool S403_AboutPageImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var homeControllerPath = Path.Combine(webRoot, "Controllers", "HomeController.cs");
        var aboutViewPath = Path.Combine(webRoot, "Views", "Home", "About.cshtml");
        var layoutPath = Path.Combine(webRoot, "Views", "Shared", "_Layout.cshtml");

        var homeControllerHasAboutAction = File.Exists(homeControllerPath)
            && File.ReadAllText(homeControllerPath).Contains("About", StringComparison.OrdinalIgnoreCase);

        var layoutHasAboutLink = File.Exists(layoutPath)
            && File.ReadAllText(layoutPath).Contains("asp-action=\"About\"", StringComparison.OrdinalIgnoreCase);

        return File.Exists(aboutViewPath)
            || homeControllerHasAboutAction
            || layoutHasAboutLink;
    }

    // S4-04 - Contact Sayfasi + Olanak Entegrasyonu karti.
    // Contact kismi ile otel detay olanak kismi ayri guard'larla izlenir.
    // Boylece var olan otel detay iskeleti CI'yi gereksiz kirmadan, yazilmaya baslanan kisim contract'a girer.
    public static bool S404_ContactPageImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var homeControllerPath = Path.Combine(webRoot, "Controllers", "HomeController.cs");
        var contactViewPath = Path.Combine(webRoot, "Views", "Home", "Contact.cshtml");
        var layoutPath = Path.Combine(webRoot, "Views", "Shared", "_Layout.cshtml");

        var homeControllerHasContactAction = File.Exists(homeControllerPath)
            && File.ReadAllText(homeControllerPath).Contains("Contact", StringComparison.OrdinalIgnoreCase);

        var layoutHasContactLink = File.Exists(layoutPath)
            && File.ReadAllText(layoutPath).Contains("asp-action=\"Contact\"", StringComparison.OrdinalIgnoreCase);

        return File.Exists(contactViewPath)
            || homeControllerHasContactAction
            || layoutHasContactLink;
    }

    public static bool S404_HotelDetailAmenitiesImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var detailViewPath = Path.Combine(webRoot, "Views", "Hotels", "Detail.cshtml");
        var detailVmPath = Path.Combine(webRoot, "Models", "HotelDetailPageVm.cs");

        var detailViewHasAmenities = File.Exists(detailViewPath)
        && (File.ReadAllText(detailViewPath).Contains("Model.Amenities", StringComparison.OrdinalIgnoreCase)
            || File.ReadAllText(detailViewPath).Contains("Olanak", StringComparison.OrdinalIgnoreCase)
            || File.ReadAllText(detailViewPath).Contains("Amenit", StringComparison.OrdinalIgnoreCase));

        var detailVmHasAmenities = File.Exists(detailVmPath)
            && File.ReadAllText(detailVmPath).Contains("Amenities", StringComparison.OrdinalIgnoreCase);

        return detailViewHasAmenities || detailVmHasAmenities;
    }

    // S4-05 - Otel Detay Entegrasyonu karti.
    // S3-05/S4-04'ten gelen temel Detail sayfasi veya olanak bolumu tek basina bu karti tamamlanmis saydirmaz.
    // Master mapping ayri, oda/yorum/booking partial entegrasyonu ayri izlenir.
    public static bool S405_HotelDetailMasterMappingImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var hotelsControllerPath = Path.Combine(webRoot, "Controllers", "HotelsController.cs");
        var detailVmPath = Path.Combine(webRoot, "Models", "HotelDetailPageVm.cs");

        if (!File.Exists(hotelsControllerPath) || !File.Exists(detailVmPath))
            return false;

        var controllerSource = File.ReadAllText(hotelsControllerPath);
        var vmSource = File.ReadAllText(detailVmPath);

        return controllerSource.Contains("Detail", StringComparison.OrdinalIgnoreCase)
            && controllerSource.Contains("HotelDetailPageVm", StringComparison.OrdinalIgnoreCase)
            && vmSource.Contains("Rooms", StringComparison.OrdinalIgnoreCase)
            && vmSource.Contains("Reviews", StringComparison.OrdinalIgnoreCase)
            && vmSource.Contains("BookingForm", StringComparison.OrdinalIgnoreCase)
            && vmSource.Contains("IsAuthenticated", StringComparison.OrdinalIgnoreCase);
    }

    public static bool S405_DetailPartialsPrepared()
    {
        var webRoot = FindWebProjectRoot();
        var candidateRoots = new[]
        {
            Path.Combine(webRoot, "Views", "Hotels"),
            Path.Combine(webRoot, "Views", "Shared")
        };

        return candidateRoots.Any(root =>
            File.Exists(Path.Combine(root, "_RoomCard.cshtml"))
            || File.Exists(Path.Combine(root, "_ReviewCard.cshtml"))
            || File.Exists(Path.Combine(root, "_BookingModal.cshtml")));
    }

    public static bool S405_DetailPartialIntegrationImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var detailViewPath = Path.Combine(webRoot, "Views", "Hotels", "Detail.cshtml");

        if (!File.Exists(detailViewPath))
            return false;

        var detailView = File.ReadAllText(detailViewPath);

        return detailView.Contains("_RoomCard", StringComparison.OrdinalIgnoreCase)
            || detailView.Contains("_ReviewCard", StringComparison.OrdinalIgnoreCase)
            || detailView.Contains("_BookingModal", StringComparison.OrdinalIgnoreCase)
            || detailView.Contains("Model.Rooms", StringComparison.OrdinalIgnoreCase)
            || detailView.Contains("Model.Reviews", StringComparison.OrdinalIgnoreCase);
    }

    // S4-06 - BookingController POST + Rezervasyonlarim karti.
    // S4-05'ten gelen booking modal veya mevcut ReservationsController.Create tek basina bu karti baslatmaz.
    // Plan S4-06 icin BookingController.Create, Reservations/My sayfasi ve iptal akislarini ayri izler.
    public static bool S406_BookingCreateImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var bookingControllerPath = Path.Combine(webRoot, "Controllers", "BookingController.cs");
        var bookingModalPath = Path.Combine(webRoot, "Views", "Hotels", "_BookingModal.cshtml");
        var modelsRoot = Path.Combine(webRoot, "Models");

        var bookingModalPostsToBooking = File.Exists(bookingModalPath)
            && File.ReadAllText(bookingModalPath).Contains("asp-controller=\"Booking\"", StringComparison.OrdinalIgnoreCase);

        var bookingModelsExist = Directory.Exists(modelsRoot)
            && Directory.GetFiles(modelsRoot, "*Booking*.cs", SearchOption.TopDirectoryOnly).Any();

        return File.Exists(bookingControllerPath)
            || bookingModalPostsToBooking
            || bookingModelsExist;
    }

    public static bool S406_UserReservationsPageImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var reservationsControllerPath = Path.Combine(webRoot, "Controllers", "ReservationsController.cs");
        var reservationsViewPath = Path.Combine(webRoot, "Views", "Reservations", "My.cshtml");
        var loginPartialPath = Path.Combine(webRoot, "Views", "Shared", "_LoginPartial.cshtml");

        var controllerSource = File.Exists(reservationsControllerPath)
            ? File.ReadAllText(reservationsControllerPath)
            : string.Empty;

        var loginPartialHasReservationsLink = File.Exists(loginPartialPath)
            && File.ReadAllText(loginPartialPath).Contains("Reservations/My", StringComparison.OrdinalIgnoreCase);

        return File.Exists(reservationsViewPath)
            || loginPartialHasReservationsLink
            || controllerSource.Contains(" My(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains(" MyAsync(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains(" Cancel(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains(" CancelAsync(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains("reservations/my", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains("/cancel", StringComparison.OrdinalIgnoreCase);
    }

    // S4-07 - Rezervasyon Detay + Yorumlarim karti.
    // S4-06'daki rezervasyon olusturma/listeme kodu tek basina bu karti baslatmaz;
    // Detail, invoice veya detay linki gorunurse S4-07 detay contract testleri devreye girer.
    public static bool S407_ReservationDetailImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var reservationsControllerPath = Path.Combine(webRoot, "Controllers", "ReservationsController.cs");
        var detailViewPath = Path.Combine(webRoot, "Views", "Reservations", "Detail.cshtml");
        var reservationsMyViewPath = Path.Combine(webRoot, "Views", "Reservations", "My.cshtml");

        var controllerSource = File.Exists(reservationsControllerPath)
            ? File.ReadAllText(reservationsControllerPath)
            : string.Empty;

        var reservationsMyView = File.Exists(reservationsMyViewPath)
            ? File.ReadAllText(reservationsMyViewPath)
            : string.Empty;

        var controllerHasDetailAction = controllerSource.Contains(" Detail(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains(" DetailAsync(", StringComparison.OrdinalIgnoreCase);

        var controllerCallsDetailEndpoint = controllerSource.Contains("reservations/{id}", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains("api/reservations/{id}", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains("reservations/" + "{reservationId}", StringComparison.OrdinalIgnoreCase);

        var controllerHasInvoiceFlow = controllerSource.Contains("reservations", StringComparison.OrdinalIgnoreCase)
            && controllerSource.Contains("invoice", StringComparison.OrdinalIgnoreCase);

        var listViewHasDetailLink = reservationsMyView.Contains("asp-action=\"Detail\"", StringComparison.OrdinalIgnoreCase)
            || reservationsMyView.Contains("Reservations/Detail", StringComparison.OrdinalIgnoreCase)
            || reservationsMyView.Contains("/Reservations/Detail", StringComparison.OrdinalIgnoreCase);

        return File.Exists(detailViewPath)
            || controllerHasDetailAction
            || controllerCallsDetailEndpoint
            || controllerHasInvoiceFlow
            || listViewHasDetailLink;
    }

    // S4-07 - Yorumlarim sayfasi ve yorum silme akisinin baslayip baslamadigini kontrol eder.
    // S3-07/S4-05'teki yorum olusturma veya otel detay yorumlari tek basina bu karti aktif etmez.
    public static bool S407_MyReviewsImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var reviewsControllerPath = Path.Combine(webRoot, "Controllers", "ReviewsController.cs");
        var reviewsMyViewPath = Path.Combine(webRoot, "Views", "Reviews", "My.cshtml");
        var loginPartialPath = Path.Combine(webRoot, "Views", "Shared", "_LoginPartial.cshtml");

        var controllerSource = File.Exists(reviewsControllerPath)
            ? File.ReadAllText(reviewsControllerPath)
            : string.Empty;

        var loginPartial = File.Exists(loginPartialPath)
            ? File.ReadAllText(loginPartialPath)
            : string.Empty;

        var controllerHasMyAction = controllerSource.Contains(" My(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains(" MyAsync(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains("GetMyReviews", StringComparison.OrdinalIgnoreCase);

        var controllerHasDeleteAction = controllerSource.Contains(" Delete(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains(" DeleteAsync(", StringComparison.OrdinalIgnoreCase)
            || controllerSource.Contains("DeleteReview", StringComparison.OrdinalIgnoreCase);

        var controllerCallsMyEndpoint = controllerSource.Contains("reviews/my", StringComparison.OrdinalIgnoreCase);

        var controllerCallsDeleteEndpoint = controllerSource.Contains("reviews/{id}", StringComparison.OrdinalIgnoreCase)
            || (controllerSource.Contains("DeleteAsync", StringComparison.OrdinalIgnoreCase)
                && controllerSource.Contains("reviews", StringComparison.OrdinalIgnoreCase));

        var loginPartialHasReviewsLink = loginPartial.Contains("Reviews/My", StringComparison.OrdinalIgnoreCase)
            || (loginPartial.Contains("asp-controller=\"Reviews\"", StringComparison.OrdinalIgnoreCase)
                && loginPartial.Contains("asp-action=\"My\"", StringComparison.OrdinalIgnoreCase));

        return File.Exists(reviewsMyViewPath)
            || controllerHasMyAction
            || controllerHasDeleteAction
            || controllerCallsMyEndpoint
            || controllerCallsDeleteEndpoint
            || loginPartialHasReviewsLink;
    }

    // S4-08 - Logger Entegrasyon + Log Endpoint'leri kartinin temel logger altyapisini kontrol eder.
    // Bu kisim S2 logger altyapisindan da gelebilir; sadece altyapi contract testlerini aktif eder.
    public static bool S408_LoggerInfrastructureImplemented()
    {
        var solutionRoot = FindSolutionRoot();
        var loggerInterfacePath = Path.Combine(solutionRoot, "HotelApp.Domain", "Services", "ILoggerService.cs");
        var loggerServicePath = Path.Combine(solutionRoot, "HotelApp.Infrastructure", "Services", "LoggerService.cs");
        var logDtoPath = Path.Combine(solutionRoot, "HotelApp.Domain", "DTOs", "LogDto.cs");
        var logEntityPath = Path.Combine(solutionRoot, "HotelApp.Domain", "Entities", "Log.cs");
        var programSource = ReadOptionalProjectFile("HotelApp.API", "Program.cs");

        return File.Exists(loggerInterfacePath)
            && File.Exists(loggerServicePath)
            && File.Exists(logDtoPath)
            && File.Exists(logEntityPath)
            && programSource.Contains("LoggerService.Initialize", StringComparison.OrdinalIgnoreCase)
            && programSource.Contains("AddSingleton", StringComparison.OrdinalIgnoreCase)
            && programSource.Contains("ILoggerService", StringComparison.OrdinalIgnoreCase);
    }

    // S4-08 - E44/E45 admin log endpoint kodu basladiysa admin log contract testleri devreye girer.
    // S4-01 admin endpointleri veya eski LoggerService altyapisi tek basina bu guard'i aktif etmez.
    public static bool S408_AdminLogEndpointsImplemented()
    {
        var source = ReadAdminControllerSource();

        return source.Contains("HttpGet(\"logs", StringComparison.OrdinalIgnoreCase)
            || source.Contains("HttpGet(\"/logs", StringComparison.OrdinalIgnoreCase)
            || source.Contains("GetLogs", StringComparison.OrdinalIgnoreCase)
            || source.Contains("GetLog", StringComparison.OrdinalIgnoreCase)
            || (source.Contains("ILoggerService", StringComparison.OrdinalIgnoreCase)
                && source.Contains("logs", StringComparison.OrdinalIgnoreCase));
    }

    // S4-08 - Auth disindaki controller'larda kritik islem loglama basladiysa entegrasyon testleri devreye girer.
    // AuthController'daki eski loglar S4-08'in tum controller entegrasyonunu baslatmis sayilmaz.
    public static bool S408_ControllerLoggingIntegrationStarted()
    {
        var controllersRoot = Path.Combine(FindSolutionRoot(), "HotelApp.API", "Controllers");
        if (!Directory.Exists(controllersRoot))
            return false;

        return Directory.GetFiles(controllersRoot, "*.cs", SearchOption.TopDirectoryOnly)
            .Where(path => !string.Equals(Path.GetFileName(path), "AuthController.cs", StringComparison.OrdinalIgnoreCase))
            .Select(File.ReadAllText)
            .Any(source => source.Contains("LogInfoAsync", StringComparison.OrdinalIgnoreCase)
                || source.Contains("LogWarningAsync", StringComparison.OrdinalIgnoreCase)
                || source.Contains("LogErrorAsync", StringComparison.OrdinalIgnoreCase));
    }

    // S4-08 - Global exception loglama entegrasyonu varsa ilgili contract testleri aktif olur.
    public static bool S408_GlobalExceptionLoggingImplemented()
    {
        var handlerSource = ReadOptionalProjectFile("HotelApp.API", "Middleware", "GlobalExceptionHandler.cs");
        var programSource = ReadOptionalProjectFile("HotelApp.API", "Program.cs");

        return handlerSource.Contains("ILoggerService", StringComparison.OrdinalIgnoreCase)
            && handlerSource.Contains("LogErrorAsync", StringComparison.OrdinalIgnoreCase)
            && programSource.Contains("AddExceptionHandler", StringComparison.OrdinalIgnoreCase)
            && programSource.Contains("UseExceptionHandler", StringComparison.OrdinalIgnoreCase);
    }

    private static bool AdminControllerSourceContains(string expected)
    {
        return ReadAdminControllerSource().Contains(expected, StringComparison.OrdinalIgnoreCase);
    }

    private static string ReadAdminControllerSource()
    {
        var controllersRoot = Path.Combine(FindSolutionRoot(), "HotelApp.API", "Controllers");
        if (!Directory.Exists(controllersRoot))
            return string.Empty;

        var adminControllerPath = Path.Combine(controllersRoot, "AdminController.cs");

        return File.Exists(adminControllerPath)
            ? File.ReadAllText(adminControllerPath)
            : string.Empty;
    }

    private static string ReadOptionalProjectFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindSolutionRoot() }.Concat(relativePathParts).ToArray());

        return File.Exists(path)
            ? File.ReadAllText(path)
            : string.Empty;
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "HotelApp.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("HotelApp.sln bulunamadi.");
    }

    private static string FindWebProjectRoot()
    {
        var candidate = Path.Combine(FindSolutionRoot(), "HotelApp.Web");

        if (Directory.Exists(candidate))
            return candidate;

        throw new DirectoryNotFoundException("HotelApp.Web klasoru bulunamadi.");
    }
}
