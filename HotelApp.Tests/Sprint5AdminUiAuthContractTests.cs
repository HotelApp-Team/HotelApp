namespace HotelApp.Tests;

// Sprint5AdminUiAuthContractTests sınıfının amacı, S5-01 Admin UI + Auth Son Kontroller kartının CI sözleşmesini korumaktır.
// Admin UI dosyaları yokken admin testleri pass geçer; dosyalar oluşunca planlanan endpoint, filtre, güvenlik ve view kurallarını doğrular.
// SOLID: SRP kullanır; sadece S5-01 kartındaki admin UI ve auth son kontrol kapsamından sorumludur.
public class Sprint5AdminUiAuthContractTests
{
    // S5-01 için admin controller'ların admin yetki koruması ve admin layout yaklaşımı kullandığını kontrol eder.
    [Fact]
    public void S501_AdminUiControllers_ShouldRequireAdminAccessWhenStarted()
    {
        // Admin UI henüz başlamadıysa CI passed geçer; dosyalar eklenince koruma zorunlu olur.
        if (!Sprint5CardGuards.S501_AdminUiImplemented())
            return;

        var sources = ReadStartedAdminControllerSources();

        Assert.NotEmpty(sources);
        foreach (var source in sources)
        {
            AssertContainsAny(source, "AdminFilter", "ServiceFilter", "Authorize", "Roles = \"Admin\"", "Roles=\"Admin\"");
            AssertContainsAny(source, "ApiService", "GetAsync", "PostAsync", "PutAsync", "DeleteAsync");
        }
    }

    // S5-01 Dashboard ekranının E36 endpointi ve plan istatistikleri ile bağlı olduğunu kontrol eder.
    [Fact]
    public void S501_DashboardUi_ShouldCallE36AndRenderPlanStatistics()
    {
        // Dashboard kısmı başlamadıysa CI passed geçer.
        if (!Sprint5CardGuards.S501_DashboardUiImplemented())
            return;

        var source = ReadAdminFeatureSource(
            controllerNames: new[] { "DashboardController.cs", "AdminDashboardController.cs" },
            viewNames: new[] { "Dashboard.cshtml", Path.Combine("Dashboard", "Index.cshtml"), Path.Combine("AdminDashboard", "Index.cshtml") });

        AssertContainsAny(source, "admin/dashboard", "api/admin/dashboard", "GetAsync");
        AssertContainsAll(source, "TotalHotels", "TotalUsers", "TotalReservations", "TotalRevenue", "AverageRating");
        AssertContainsAny(source, "ActiveReservations", "Aktif Rezervasyon");
        AssertContainsAny(source, "RecentReservations", "Son Rezervasyon", "recent");
        AssertContainsAny(source, "_AdminLayout", "Layout", "AdminFilter", "Authorize");
    }

    // S5-01 Kullanıcı yönetimi ekranının E37-E39 endpointleri ve güvenli alan kullanımıyla bağlı olduğunu kontrol eder.
    [Fact]
    public void S501_UserMgmtUi_ShouldCallE37E38E39AndAvoidSensitiveData()
    {
        // Kullanıcı yönetimi kısmı başlamadıysa CI passed geçer.
        if (!Sprint5CardGuards.S501_UserMgmtUiImplemented())
            return;

        var source = ReadAdminFeatureSource(
            controllerNames: new[] { "UserMgmtController.cs", "UsersController.cs", "AdminUsersController.cs", "AdminUserMgmtController.cs" },
            viewNames: new[] { "Users.cshtml", Path.Combine("Users", "Index.cshtml"), Path.Combine("UserMgmt", "Index.cshtml"), Path.Combine("AdminUserMgmt", "Index.cshtml"), Path.Combine("AdminUsers", "Index.cshtml") });

        AssertContainsAny(source, "admin/users", "api/admin/users");
        AssertContainsAny(source, "search", "Search");
        AssertContainsAny(source, "isActive", "IsActive", "Aktif");
        AssertContainsAny(source, "page", "PageNumber", "CurrentPage");
        AssertContainsAny(source, "pageSize", "PageSize");
        AssertContainsAny(source, "users/{id}/status", "/status", "UpdateStatus", "IsActive");
        AssertContainsAny(source, "users/{id}/roles", "/roles", "UpdateRoles", "Roles");
        AssertContainsAny(source, "TempData", "ModelState", "alert", "ErrorMessage", "SuccessMessage");
        Assert.DoesNotContain("PasswordHash", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("EmailVerificationToken", source, StringComparison.OrdinalIgnoreCase);
    }

    // S5-01 Log ekranının E44-E45 endpointleri, filtreleri ve detay davranışıyla bağlı olduğunu kontrol eder.
    [Fact]
    public void S501_LogUi_ShouldCallE44E45WithFiltersAndSafeDetailBehavior()
    {
        // Log ekranı başlamadıysa CI passed geçer.
        if (!Sprint5CardGuards.S501_LogUiImplemented())
            return;

        var source = ReadAdminFeatureSource(
            controllerNames: new[] { "LogController.cs", "LogsController.cs", "AdminLogsController.cs", "AdminLogController.cs" },
            viewNames: new[] { "Logs.cshtml", Path.Combine("Logs", "Index.cshtml"), Path.Combine("Log", "Index.cshtml"), Path.Combine("AdminLog", "Index.cshtml") });

        AssertContainsAny(source, "admin/logs", "api/admin/logs");
        AssertContainsAll(source, "level", "dateFrom", "dateTo", "search");
        AssertContainsAny(source, "page", "PageNumber", "CurrentPage");
        AssertContainsAny(source, "pageSize", "PageSize");
        AssertContainsAny(source, "logs/{id}", "admin/logs/", "Detail", "modal");
        AssertContainsAny(source, "ExceptionDetail", "Hata Detayı", "Detay");
        AssertContainsAny(source, "TempData", "ModelState", "alert", "ErrorMessage", "SuccessMessage");
    }

    // S5-01 auth son kontrol kısmında RememberMe, email verify/resend ve lockout sözleşmesinin korunduğunu kontrol eder.
    [Fact]
    public void S501_AuthFinalControls_ShouldKeepRememberMeVerifyResendAndLockoutContracts()
    {
        var webAuthController = ReadWebFile("Controllers", "AuthController.cs");
        var loginView = ReadWebFile("Views", "Auth", "Login.cshtml");
        var loginViewModel = ReadWebFile("Models", "LoginViewModel.cs");
        var authService = ReadProjectFile("HotelApp.Domain", "Services", "AuthService.cs");
        var jwtService = ReadProjectFile("HotelApp.Domain", "Services", "JwtService.cs");

        AssertContainsAll(loginViewModel, "RememberMe", "ReturnUrl");
        AssertContainsAny(loginView, "asp-for=\"RememberMe\"", "Beni Hatırla", "Beni Hatirla");
        AssertContainsAll(webAuthController, "RememberMe", "auth/login", "ReturnUrl", "Url.IsLocalUrl");
        AssertContainsAll(authService, "FailedLoginAttempts", "LockoutEndTime", "RememberMe");
        AssertContainsAny(authService, ">= 5", ">=5", "FailedLoginAttempts >= 5");
        AssertContainsAny(authService, "AddMinutes(1", "AddMinutes( 1");
        AssertContainsAny(authService, "FailedLoginAttempts = 0", "FailedLoginAttempts=0");
        AssertContainsAny(authService, "LockoutEndTime = null", "LockoutEndTime=null");
        AssertContainsAll(authService, "VerifyEmailAsync", "ResendVerificationAsync", "EmailVerificationToken", "VerificationTokenExpiry");
        AssertContainsAll(jwtService, "rememberMe", "AddDays", "_rememberMeTokenExpiryDays", "AddMinutes", "_expirationMinutes");
    }

    // S5-01 admin UI başladıysa public layout yerine admin layout izinin korunmasını kontrol eder.
    [Fact]
    public void S501_AdminViews_ShouldUseAdminLayoutWhenStarted()
    {
        // Admin UI henüz başlamadıysa CI passed geçer.
        if (!Sprint5CardGuards.S501_AdminUiImplemented())
            return;

        var viewSource = string.Join(Environment.NewLine, ReadStartedAdminViewSources());

        Assert.NotEmpty(viewSource);
        AssertContainsAny(viewSource, "_AdminLayout", "Layout =", "Admin");
        Assert.DoesNotContain("PasswordHash", viewSource, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", viewSource, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("EmailVerificationToken", viewSource, StringComparison.OrdinalIgnoreCase);
    }

    // Başlamış admin controller kaynaklarını bir araya getirir.
    private static List<string> ReadStartedAdminControllerSources()
    {
        var sources = new List<string>();

        // İlgili admin özelliği başladıysa o controller dosyalarını test kapsamına alırız.
        if (Sprint5CardGuards.S501_DashboardUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminControllerSources("DashboardController.cs", "AdminDashboardController.cs"));

        if (Sprint5CardGuards.S501_UserMgmtUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminControllerSources("UserMgmtController.cs", "UsersController.cs", "AdminUsersController.cs"));

        if (Sprint5CardGuards.S501_LogUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminControllerSources("LogController.cs", "LogsController.cs", "AdminLogsController.cs"));

        return sources.ToList();
    }

    // Başlamış admin view kaynaklarını bir araya getirir.
    private static List<string> ReadStartedAdminViewSources()
    {
        var sources = new List<string>();

        // Hangi admin ekranı başladıysa yalnızca onun view dosyalarını okuruz.
        if (Sprint5CardGuards.S501_DashboardUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminViewSources("Dashboard.cshtml", Path.Combine("Dashboard", "Index.cshtml"), Path.Combine("AdminDashboard", "Index.cshtml")));

        if (Sprint5CardGuards.S501_UserMgmtUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminViewSources("Users.cshtml", Path.Combine("Users", "Index.cshtml"), Path.Combine("UserMgmt", "Index.cshtml"), Path.Combine("AdminUserMgmt", "Index.cshtml"), Path.Combine("AdminUsers", "Index.cshtml")));

        if (Sprint5CardGuards.S501_LogUiImplemented())
            sources.AddRange(Sprint5CardGuards.CandidateAdminViewSources("Logs.cshtml", Path.Combine("Logs", "Index.cshtml"), Path.Combine("Log", "Index.cshtml"), Path.Combine("AdminLog", "Index.cshtml")));

        return sources.ToList();
    }

    // Bir admin özelliğine ait controller ve view kaynaklarını tek metinde toplar.
    private static string ReadAdminFeatureSource(string[] controllerNames, string[] viewNames)
    {
        var controllerSources = Sprint5CardGuards.CandidateAdminControllerSources(controllerNames);
        var viewSources = Sprint5CardGuards.CandidateAdminViewSources(viewNames);

        return string.Join(Environment.NewLine, controllerSources.Concat(viewSources));
    }

    // Metin içinde beklenen tüm parçaların geçtiğini kontrol eder.
    private static void AssertContainsAll(string text, params string[] expectedParts)
    {
        foreach (var expected in expectedParts)
        {
            Assert.Contains(expected, text, StringComparison.OrdinalIgnoreCase);
        }
    }

    // Metin içinde beklenen parçalardan en az birinin geçtiğini kontrol eder.
    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase)),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }

    // Web projesindeki bir dosyayı okur.
    private static string ReadWebFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.Web") }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    // Verilen proje klasöründeki bir dosyayı okur.
    private static string ReadProjectFile(string projectName, params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot(projectName) }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    // Test çalışma klasöründen yukarı çıkarak istenen proje klasörünü bulur.
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
}
