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
            || CandidateAdminViewSources("Dashboard.cshtml", Path.Combine("Dashboard", "Index.cshtml"), Path.Combine("AdminDashboard", "Index.cshtml"))
                .Any(source => ContainsAny(source, "TotalHotels", "Toplam Otel", "Dashboard"));
    }

    // S5-01 Kullanıcı yönetimi kısmı başladıysa Users contract testleri aktif olur.
    public static bool S501_UserMgmtUiImplemented()
    {
        return CandidateAdminControllerSources("UserMgmtController.cs", "UsersController.cs", "AdminUsersController.cs", "AdminUserMgmtController.cs")
                .Any(source => ContainsAny(source, "admin/users", "api/admin/users", "UserMgmt", "UpdateUserStatus", "UpdateUserRoles"))
            || CandidateAdminViewSources("Users.cshtml", Path.Combine("Users", "Index.cshtml"), Path.Combine("UserMgmt", "Index.cshtml"), Path.Combine("AdminUserMgmt", "Index.cshtml"), Path.Combine("AdminUsers", "Index.cshtml"))
                .Any(source => ContainsAny(source, "Kullanıcı", "Users", "IsActive", "Roles"));
    }

    // S5-01 Log ekranı başladıysa Logs contract testleri aktif olur.
    public static bool S501_LogUiImplemented()
    {
        return CandidateAdminControllerSources("LogController.cs", "LogsController.cs", "AdminLogsController.cs", "AdminLogController.cs")
                .Any(source => ContainsAny(source, "admin/logs", "api/admin/logs", "LogController", "GetLog"))
            || CandidateAdminViewSources("Logs.cshtml", Path.Combine("Logs", "Index.cshtml"), Path.Combine("Log", "Index.cshtml"), Path.Combine("AdminLog", "Index.cshtml"))
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
            || CandidateAdminViewSources("Hotels.cshtml", Path.Combine("Hotels", "Index.cshtml"), Path.Combine("HotelMgmt", "Index.cshtml"), Path.Combine("AdminHotels", "Index.cshtml"))
                .Any(source => ContainsAny(source, "Otel", "Hotels", "StarRating", "City", "Delete"));
    }

    // S5-03 Admin otel düzenleme/görsel/olanak atama ekranı başladıysa ilgili contract testleri aktif olur.
    public static bool S503_HotelEditUiImplemented()
    {
        return CandidateAdminControllerSources("HotelMgmtController.cs", "AdminHotelsController.cs")
                .Any(source => ContainsAny(source, "HotelEdit", "Edit(", "images", "amenities", "HotelAmenities"))
            || CandidateAdminViewSources("HotelEdit.cshtml", Path.Combine("Hotels", "Edit.cshtml"), Path.Combine("HotelMgmt", "Edit.cshtml"), Path.Combine("AdminHotels", "Edit.cshtml"))
                .Any(source => ContainsAny(source, "HotelEdit", "Görsel", "Gorsel", "Image", "Amenit", "Olanak"));
    }

    // S5-03 Admin olanak yönetimi ekranı başladıysa ilgili contract testleri aktif olur.
    // Not: AmenityMgmtController/AmenityMgmt.cshtml refactor sonrasi eklendi
    // (eski Amenities.cshtml ve RoomAmenities.cshtml tek sayfada birlestirildi).
    public static bool S503_AmenityUiImplemented()
    {
        return CandidateAdminControllerSources("AmenityController.cs", "AmenitiesController.cs", "AdminAmenitiesController.cs", "AmenityMgmtController.cs")
                .Any(source => ContainsAny(source, "Amenity", "Amenities", "api/amenities", "DeleteAsync"))
            || CandidateAdminViewSources("Amenities.cshtml", Path.Combine("Amenities", "Index.cshtml"), Path.Combine("Amenity", "Index.cshtml"), "AmenityMgmt.cshtml", Path.Combine("AdminAmenities", "Index.cshtml"))
                .Any(source => ContainsAny(source, "Olanak", "Amenity", "Amenities", "Icon"));
    }

    // S5-03 kartındaki herhangi bir admin otel/olanak ekranı başladı mı kontrol eder.
    public static bool S503_AdminHotelAmenitiesUiImplemented()
    {
        return S503_HotelMgmtUiImplemented()
            || S503_HotelEditUiImplemented()
            || S503_AmenityUiImplemented();
    }

    // S5-04 public otel akışı başladıysa detail veya arama contract testleri aktif olur.
    public static bool S504_PublicHotelFlowImplemented()
    {
        return S504_DetailPageImplemented()
            || S504_SearchFilterUiImplemented();
    }

    // S5-04 otel detay polish kapsamındaki Detail action/view/model başladı mı kontrol eder.
    public static bool S504_DetailPageImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "HotelsController.cs");
        var detailViewPath = Path.Combine(webRoot, "Views", "Hotels", "Detail.cshtml");
        var modelPath = Path.Combine(webRoot, "Models", "HotelDetailPageVm.cs");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;
        var viewSource = File.Exists(detailViewPath) ? File.ReadAllText(detailViewPath) : string.Empty;

        return File.Exists(modelPath)
            || ContainsAny(controllerSource, "Detail(", "HotelDetailPageVm", "hotels/{id}", "rooms", "reviews")
            || ContainsAny(viewSource, "HotelDetailPageVm", "Galeri", "Olanak", "_RoomCard", "_ReviewCard");
    }

    // S5-04 arama/filtre bug fix kapsamındaki Search action/view başladı mı kontrol eder.
    public static bool S504_SearchFilterUiImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "HotelsController.cs");
        var searchViewPath = Path.Combine(webRoot, "Views", "Hotels", "Search.cshtml");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;
        var viewSource = File.Exists(searchViewPath) ? File.ReadAllText(searchViewPath) : string.Empty;

        return ContainsAny(controllerSource, "Search(", "hotels/search", "checkIn", "guestCount", "sortBy", "pageSize")
            || ContainsAny(viewSource, "sortBy", "guestCount", "pagination", "_HotelCard", "asp-route-page");
    }

    // S5-04 demo seed data dosyası eklendiyse seed contract testi aktif olur.
    public static bool S504_DemoSeedDataImplemented()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var solutionFile = Path.Combine(directory.FullName, "HotelApp.sln");
            if (File.Exists(solutionFile))
            {
                return Directory.GetFiles(directory.FullName, "*.sql", SearchOption.AllDirectories)
                    .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                        && !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                        && !path.Contains($"{Path.DirectorySeparatorChar}HotelApp.Tests{Path.DirectorySeparatorChar}"))
                    .Any(path => ContainsAny(File.ReadAllText(path), "INSERT INTO Hotels", "INSERT [dbo].[Hotels]", "HotelImages", "Amenities"));
            }

            directory = directory.Parent;
        }

        return false;
    }

    // S5-05 RoomTypes API dosyası varsa E20-E22/E51 contract testleri aktif olur.
    public static bool S505_RoomTypesApiImplemented()
    {
        var apiRoot = FindApiProjectRoot();
        var controllerSources = new[]
        {
            Path.Combine(apiRoot, "Controllers", "RoomTypeController.cs"),
            Path.Combine(apiRoot, "Controllers", "RoomTypesController.cs")
        };

        return controllerSources
            .Where(File.Exists)
            .Any(path => ContainsAny(File.ReadAllText(path), "api/roomtypes", "RoomTypeController", "RoomTypesController", "HttpDelete"));
    }

    // S5-05 Admin RoomTypes UI başladıysa ilgili contract testleri aktif olur.
    public static bool S505_AdminRoomTypesUiImplemented()
    {
        return CandidateAdminControllerSources("RoomTypeController.cs", "RoomTypesController.cs", "AdminRoomTypesController.cs")
                .Any(source => ContainsAny(source, "api/roomtypes", "RoomTypes", "RoomType", "DeleteAsync"))
            || CandidateAdminViewSources("RoomTypes.cshtml", Path.Combine("RoomTypes", "Index.cshtml"), Path.Combine("RoomType", "Index.cshtml"), Path.Combine("AdminRoomTypes", "Index.cshtml"))
                .Any(source => ContainsAny(source, "RoomTypes", "RoomType", "Oda tipi", "MaxGuests"));
    }

    // S5-05 Admin rezervasyon API endpointleri başladıysa E40-E41/E48 contract testleri aktif olur.
    public static bool S505_AdminReservationsApiImplemented()
    {
        var apiRoot = FindApiProjectRoot();
        var adminController = Path.Combine(apiRoot, "Controllers", "AdminController.cs");
        var reservationAdminController = Path.Combine(apiRoot, "Controllers", "AdminReservationsController.cs");

        var sources = new[] { adminController, reservationAdminController }
            .Where(File.Exists)
            .Select(File.ReadAllText);

        return sources.Any(source => ContainsAny(source,
            "admin/reservations",
            "HttpGet(\"reservations\")",
            "HttpPut(\"reservations/{id}/status\")",
            "HttpPut(\"reservations/{id}/confirm\")",
            "UpdateReservationStatus",
            "ConfirmReservation"));
    }

    // S5-05 Admin Reservations UI başladıysa ilgili contract testleri aktif olur.
    public static bool S505_AdminReservationsUiImplemented()
    {
        return CandidateAdminControllerSources("ReservationMgmtController.cs", "ReservationsController.cs", "AdminReservationsController.cs")
                .Any(source => ContainsAny(source, "admin/reservations", "ReservationMgmt", "UpdateStatus", "Confirm", "Reject"))
            || CandidateAdminViewSources("Reservations.cshtml", Path.Combine("Reservations", "Index.cshtml"), Path.Combine("ReservationMgmt", "Index.cshtml"), Path.Combine("AdminReservations", "Index.cshtml"))
                .Any(source => ContainsAny(source, "Rezervasyon", "Reservation", "Status", "dateFrom", "Confirm"));
    }

    // S5-05 FAQ sayfası başladıysa public FAQ contract testi aktif olur.
    public static bool S505_FaqPageImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "HomeController.cs");
        var viewPath = Path.Combine(webRoot, "Views", "Home", "Faq.cshtml");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;
        var viewSource = File.Exists(viewPath) ? File.ReadAllText(viewPath) : string.Empty;

        return ContainsAny(controllerSource, "Faq(", "FAQ(")
            || ContainsAny(viewSource, "accordion", "FAQ", "Sıkça", "Sikca");
    }

    // S5-05 admin oda tipi veya rezervasyon UI dosyaları başladı mı kontrol eder.
    public static bool S505_AdminRoomReservationUiImplemented()
    {
        return S505_AdminRoomTypesUiImplemented()
            || S505_AdminReservationsUiImplemented();
    }

    // S5-06 rezervasyon API dosyası varsa E23-E27 bug fix contract testleri aktif olur.
    public static bool S506_ReservationApiImplemented()
    {
        var apiRoot = FindApiProjectRoot();
        var sources = new[]
        {
            Path.Combine(apiRoot, "Controllers", "ReservationController.cs"),
            Path.Combine(apiRoot, "Controllers", "ReservationsController.cs")
        };

        return sources
            .Where(File.Exists)
            .Any(path => ContainsAny(File.ReadAllText(path), "api/reservations", "HttpPost", "HttpGet(\"my\")", "cancel", "invoice"));
    }

    // S5-06 yorum API dosyası varsa E28-E31 bug fix contract testleri aktif olur.
    public static bool S506_ReviewApiImplemented()
    {
        var apiRoot = FindApiProjectRoot();
        var sources = new[]
        {
            Path.Combine(apiRoot, "Controllers", "ReviewController.cs"),
            Path.Combine(apiRoot, "Controllers", "ReviewsController.cs")
        };

        return sources
            .Where(File.Exists)
            .Any(path => ContainsAny(File.ReadAllText(path), "api/reviews", "hotels/{hotelId}/reviews", "HttpGet(\"my\")", "HttpDelete"));
    }

    // S5-06 otel detayindan rezervasyon olusturma UI akisi başladı mı kontrol eder.
    public static bool S506_ReservationCreateUiImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "ReservationsController.cs");
        var bookingModalPath = Path.Combine(webRoot, "Views", "Hotels", "_BookingModal.cshtml");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;
        var viewSource = File.Exists(bookingModalPath) ? File.ReadAllText(bookingModalPath) : string.Empty;

        return ContainsAny(controllerSource, "Create(", "PostAsync", "reservations")
            || ContainsAny(viewSource, "Reservations", "Create", "HotelRoomId", "CheckInDate");
    }

    // S5-06 kullanici rezervasyon sayfalari başladı mı kontrol eder.
    public static bool S506_ReservationUserPagesImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "ReservationsController.cs");
        var myViewPath = Path.Combine(webRoot, "Views", "Reservations", "My.cshtml");
        var detailViewPath = Path.Combine(webRoot, "Views", "Reservations", "Detail.cshtml");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;

        return File.Exists(myViewPath)
            || File.Exists(detailViewPath)
            || ContainsAny(controllerSource, " My(", " Detail(", " Cancel(", "Invoice", "reservations/my", "/cancel", "/invoice");
    }

    // S5-06 otel detayindan yorum ekleme UI akisi başladı mı kontrol eder.
    public static bool S506_ReviewCreateUiImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "ReviewsController.cs");
        var detailViewPath = Path.Combine(webRoot, "Views", "Hotels", "Detail.cshtml");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;
        var viewSource = File.Exists(detailViewPath) ? File.ReadAllText(detailViewPath) : string.Empty;

        return ContainsAny(controllerSource, "Create(", "PostAsync", "reviews")
            || ContainsAny(viewSource, "Reviews", "Create", "Rating", "Comment");
    }

    // S5-06 kullanicinin kendi yorumlari/silme UI akisi başladı mı kontrol eder.
    public static bool S506_ReviewUserPagesImplemented()
    {
        var webRoot = FindWebProjectRoot();
        var controllerPath = Path.Combine(webRoot, "Controllers", "ReviewsController.cs");
        var myViewPath = Path.Combine(webRoot, "Views", "Reviews", "My.cshtml");

        var controllerSource = File.Exists(controllerPath) ? File.ReadAllText(controllerPath) : string.Empty;

        return File.Exists(myViewPath)
            || ContainsAny(controllerSource, " My(", " Delete(", "reviews/my", "DeleteAsync");
    }

    // S5-06 rezervasyon veya yorum akisi başladı mı kontrol eder.
    public static bool S506_ReservationReviewFlowImplemented()
    {
        return S506_ReservationApiImplemented()
            || S506_ReviewApiImplemented()
            || S506_ReservationCreateUiImplemented()
            || S506_ReservationUserPagesImplemented()
            || S506_ReviewCreateUiImplemented()
            || S506_ReviewUserPagesImplemented();
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

    // Test çalışma klasöründen yukarı çıkarak API projesinin gerçek yolunu bulur.
    private static string FindApiProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "HotelApp.API");
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("HotelApp.API klasoru bulunamadi.");
    }
}
