namespace HotelApp.Tests;

// Sprint5CardGuards sınıfının amacı, Sprint 5 kartları için şartlı CI testlerinin ne zaman devreye gireceğini belirlemektir.
// Henüz başlanmamış kartlarda testler pass geçer; ilgili controller/view dosyaları oluşunca contract testleri çalışır.
// SOLID: SRP kullanır; sadece Sprint 5 kart başlangıç sinyallerini kontrol eder.
internal static class Sprint5CardGuards
{
    // S5-01 Admin UI kartının herhangi bir admin ekranı ile başlayıp başlamadığını kontrol eder.
    public static bool S501_AdminUiImplemented()
    {
        return S501_DashboardUiImplemented()
            || S501_UserMgmtUiImplemented()
            || S501_LogUiImplemented();
    }

    // S5-01 Dashboard kısmı başladıysa Dashboard contract testleri aktif olur.
    public static bool S501_DashboardUiImplemented()
    {
        return CandidateAdminControllerSources("DashboardController.cs", "AdminDashboardController.cs")
                .Any(source => ContainsAny(source, "Dashboard", "admin/dashboard", "api/admin/dashboard"))
            || CandidateAdminViewSources("Dashboard.cshtml", Path.Combine("Dashboard", "Index.cshtml"))
                .Any(source => ContainsAny(source, "TotalHotels", "Toplam Otel", "Dashboard"));
    }

    // S5-01 Kullanıcı yönetimi kısmı başladıysa Users contract testleri aktif olur.
    public static bool S501_UserMgmtUiImplemented()
    {
        return CandidateAdminControllerSources("UserMgmtController.cs", "UsersController.cs", "AdminUsersController.cs")
                .Any(source => ContainsAny(source, "admin/users", "api/admin/users", "UserMgmt", "UpdateUserStatus", "UpdateUserRoles"))
            || CandidateAdminViewSources("Users.cshtml", Path.Combine("Users", "Index.cshtml"), Path.Combine("UserMgmt", "Index.cshtml"))
                .Any(source => ContainsAny(source, "Kullanıcı", "Users", "IsActive", "Roles"));
    }

    // S5-01 Log ekranı başladıysa Logs contract testleri aktif olur.
    public static bool S501_LogUiImplemented()
    {
        return CandidateAdminControllerSources("LogController.cs", "LogsController.cs", "AdminLogsController.cs")
                .Any(source => ContainsAny(source, "admin/logs", "api/admin/logs", "LogController", "GetLog"))
            || CandidateAdminViewSources("Logs.cshtml", Path.Combine("Logs", "Index.cshtml"), Path.Combine("Log", "Index.cshtml"))
                .Any(source => ContainsAny(source, "Log", "Level", "ExceptionDetail", "dateFrom"));
    }

    // S5-02 Auth Bug Fix + Cross-Test kartının auth hedefleri mevcutsa testleri aktif eder.
    public static bool S502_AuthCrossTestTargetsImplemented()
    {
        var webRoot = FindWebProjectRoot();

        return File.Exists(Path.Combine(webRoot, "Controllers", "AuthController.cs"))
            || File.Exists(Path.Combine(webRoot, "Filters", "AuthFilter.cs"))
            || File.Exists(Path.Combine(webRoot, "Filters", "AdminFilter.cs"))
            || File.Exists(Path.Combine(webRoot, "Services", "ApiService.cs"))
            || File.Exists(Path.Combine(webRoot, "Views", "Shared", "_LoginPartial.cshtml"));
    }

    // S5-02 kapsamında kullanıcıya özel rezervasyon sayfaları başladı mı kontrol eder.
    public static bool S502_ReservationUserPagesImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "ReservationsController.cs");
        var myViewPath = Path.Combine(webRoot, "Views", "Reservations", "My.cshtml");
        var detailViewPath = Path.Combine(webRoot, "Views", "Reservations", "Detail.cshtml");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;

        return File.Exists(myViewPath)
            || File.Exists(detailViewPath)
            || ContainsAny(controllerSource, " My(", " MyAsync(", " Detail(", " DetailAsync(", "reservations/my", "reservations/{id}");
    }

    // S5-02 kapsamında kullanıcıya özel yorumlarım sayfası başladı mı kontrol eder.
    public static bool S502_ReviewsUserPagesImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "ReviewsController.cs");
        var myViewPath = Path.Combine(webRoot, "Views", "Reviews", "My.cshtml");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;

        return File.Exists(myViewPath)
            || ContainsAny(controllerSource, " My(", " MyAsync(", "Delete(", "DeleteAsync(", "reviews/my");
    }

    // S5-03 Admin otel yönetimi ekranı başladıysa ilgili contract testleri aktif olur.
    public static bool S503_HotelMgmtUiImplemented()
    {
        return CandidateAdminControllerSources("HotelMgmtController.cs", "AdminHotelsController.cs")
                .Any(source => ContainsAny(source, "HotelMgmt", "admin/hotels", "api/hotels", "DeleteAsync", "HotelEdit"))
            || CandidateAdminViewSources("Hotels.cshtml", Path.Combine("Hotels", "Index.cshtml"), Path.Combine("HotelMgmt", "Index.cshtml"))
                .Any(source => ContainsAny(source, "Otel", "Hotels", "StarRating", "City", "Delete"));
    }

    // S5-03 Admin otel düzenleme/görsel/olanak atama ekranı başladıysa ilgili contract testleri aktif olur.
    public static bool S503_HotelEditUiImplemented()
    {
        return CandidateAdminControllerSources("HotelMgmtController.cs", "AdminHotelsController.cs")
                .Any(source => ContainsAny(source, "HotelEdit", "Edit(", "images", "amenities", "HotelAmenities"))
            || CandidateAdminViewSources("HotelEdit.cshtml", Path.Combine("Hotels", "Edit.cshtml"), Path.Combine("HotelMgmt", "Edit.cshtml"))
                .Any(source => ContainsAny(source, "HotelEdit", "Görsel", "Gorsel", "Image", "Amenit", "Olanak"));
    }

    // S5-03 Admin olanak yönetimi ekranı başladıysa ilgili contract testleri aktif olur.
    public static bool S503_AmenityUiImplemented()
    {
        return CandidateAdminControllerSources("AmenityController.cs", "AmenitiesController.cs", "AdminAmenitiesController.cs")
                .Any(source => ContainsAny(source, "Amenity", "Amenities", "api/amenities", "DeleteAsync"))
            || CandidateAdminViewSources("Amenities.cshtml", Path.Combine("Amenities", "Index.cshtml"), Path.Combine("Amenity", "Index.cshtml"))
                .Any(source => ContainsAny(source, "Olanak", "Amenity", "Amenities", "Icon"));
    }

    // S5-03 kartındaki herhangi bir admin otel/olanak ekranı başladı mı kontrol eder.
    public static bool S503_AdminHotelAmenitiesUiImplemented()
    {
        return S503_HotelMgmtUiImplemented()
            || S503_HotelEditUiImplemented()
            || S503_AmenityUiImplemented();
    }

    // Admin controller kaynaklarını olası klasör yapıları üzerinden okur.
    public static IEnumerable<string> CandidateAdminControllerSources(params string[] fileNames)
    {
        var webRoot = FindWebProjectRoot();
        var candidateRoots = new[]
        {
            Path.Combine(webRoot, "Controllers", "Admin"),
            Path.Combine(webRoot, "Areas", "Admin", "Controllers"),
            Path.Combine(webRoot, "Controllers")
        };

        return candidateRoots
            .Where(Directory.Exists)
            .SelectMany(root => fileNames.Select(fileName => Path.Combine(root, fileName)))
            .Where(File.Exists)
            .Select(File.ReadAllText);
    }

    // Admin view kaynaklarını olası klasör yapıları üzerinden okur.
    public static IEnumerable<string> CandidateAdminViewSources(params string[] relativePaths)
    {
        var webRoot = FindWebProjectRoot();
        var candidateRoots = new[]
        {
            Path.Combine(webRoot, "Views", "Admin"),
            Path.Combine(webRoot, "Areas", "Admin", "Views"),
            Path.Combine(webRoot, "Views")
        };

        return candidateRoots
            .Where(Directory.Exists)
            .SelectMany(root => relativePaths.Select(relativePath => Path.Combine(root, relativePath)))
            .Where(File.Exists)
            .Select(File.ReadAllText);
    }

    // Metin içinde beklenen parçalardan en az biri var mı kontrol eder.
    private static bool ContainsAny(string text, params string[] expectedParts)
    {
        return expectedParts.Any(part => text.Contains(part, StringComparison.OrdinalIgnoreCase));
    }

    // Test çalışma klasöründen yukarı çıkarak Web projesinin gerçek yolunu bulur.
    private static string FindWebProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "HotelApp.Web");
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("HotelApp.Web klasoru bulunamadi.");
    }
}
