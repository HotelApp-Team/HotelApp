using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebAuthController = HotelApp.Web.Controllers.AuthController;

namespace HotelApp.Tests;

// Sprint5AuthCrossTestContractTests sınıfının amacı, S5-02 Auth Bug Fix + Cross-Test kartının CI sözleşmesini korumaktır.
// Auth altyapısı mevcutsa login/logout, token/session, 401/403, navbar ve kullanıcı/admin erişim kontrollerini doğrular.
// SOLID: SRP kullanır; sadece S5-02 auth stabilizasyon ve cross-test kurallarından sorumludur.
public class Sprint5AuthCrossTestContractTests
{
    // S5-02 için AuthFilter ve AdminFilter'ın anonim/user/admin erişim ayrımını doğru yaptığını kontrol eder.
    [Fact]
    public void S502_AuthFilters_ShouldRedirectAnonymousAndBlockUserRoleFromAdminPages()
    {
        // Auth hedefleri yoksa CI passed geçer; auth altyapısı varsa contract aktif olur.
        if (!Sprint5CardGuards.S502_AuthCrossTestTargetsImplemented())
            return;

        var authContext = CreateExecutingContext(new TestSession());
        new AuthFilter().OnActionExecuting(authContext);

        var authRedirect = Assert.IsType<RedirectToActionResult>(authContext.Result);
        Assert.Equal("Login", authRedirect.ActionName);
        Assert.Equal("Auth", authRedirect.ControllerName);

        var anonymousAdminContext = CreateExecutingContext(new TestSession());
        new AdminFilter().OnActionExecuting(anonymousAdminContext);

        var adminLoginRedirect = Assert.IsType<RedirectToActionResult>(anonymousAdminContext.Result);
        Assert.Equal("Login", adminLoginRedirect.ActionName);
        Assert.Equal("Auth", adminLoginRedirect.ControllerName);

        var userSession = new TestSession();
        userSession.SetString("JWToken", CreateJwtToken(DateTime.UtcNow.AddMinutes(30), "User"));
        var userAdminContext = CreateExecutingContext(userSession);
        new AdminFilter().OnActionExecuting(userAdminContext);

        var homeRedirect = Assert.IsType<RedirectToActionResult>(userAdminContext.Result);
        Assert.Equal("Index", homeRedirect.ActionName);
        Assert.Equal("Home", homeRedirect.ControllerName);

        var adminSession = new TestSession();
        adminSession.SetString("JWToken", CreateJwtToken(DateTime.UtcNow.AddMinutes(30), "Admin"));
        var validAdminContext = CreateExecutingContext(adminSession);
        new AdminFilter().OnActionExecuting(validAdminContext);

        Assert.Null(validAdminContext.Result);
    }

    // S5-02 için ApiService'in token eklediğini ve 401/403 cevaplarını exception patlatmadan kullanıcı mesajına çevirdiğini kontrol eder.
    [Fact]
    public async Task S502_ApiService_ShouldAttachBearerTokenAndHandle401403Responses()
    {
        // Auth hedefleri yoksa CI passed geçer; auth altyapısı varsa contract aktif olur.
        if (!Sprint5CardGuards.S502_AuthCrossTestTargetsImplemented())
            return;

        var unauthorizedHandler = new CapturingHttpMessageHandler(_ => JsonResponse(
            HttpStatusCode.Unauthorized,
            new { success = false, message = "Lütfen giriş yapın." }));
        var session = new TestSession();
        session.SetString("JWToken", "session-token");
        var unauthorizedService = CreateApiService(unauthorizedHandler, session);

        var unauthorizedResult = await unauthorizedService.GetAsync<object>("auth/profile");

        Assert.False(unauthorizedResult.Success);
        Assert.Equal(401, unauthorizedResult.StatusCode);
        Assert.Equal("Lütfen giriş yapın.", unauthorizedResult.Message);
        Assert.Equal("Bearer", unauthorizedHandler.LastRequest?.Headers.Authorization?.Scheme);
        Assert.Equal("session-token", unauthorizedHandler.LastRequest?.Headers.Authorization?.Parameter);

        var forbiddenHandler = new CapturingHttpMessageHandler(_ => JsonResponse(
            HttpStatusCode.Forbidden,
            new { success = false, message = "Bu sayfa için yetkiniz yok." }));
        var forbiddenService = CreateApiService(forbiddenHandler, session);

        var forbiddenResult = await forbiddenService.GetAsync<object>("admin/dashboard");

        Assert.False(forbiddenResult.Success);
        Assert.Equal(403, forbiddenResult.StatusCode);
        Assert.Equal("Bu sayfa için yetkiniz yok.", forbiddenResult.Message);
    }

    // S5-02 için VerifyEmail tokenının gerçekten API'ye taşındığını ve logout'un session temizlediğini kontrol eder.
    [Fact]
    public async Task S502_AuthController_ShouldSendRealVerifyEmailTokenAndClearSessionOnLogout()
    {
        // Auth hedefleri yoksa CI passed geçer; auth altyapısı varsa contract aktif olur.
        if (!Sprint5CardGuards.S502_AuthCrossTestTargetsImplemented())
            return;

        var response = new LoginResponse
        {
            Token = "verify-jwt-token",
            Email = "verified@hotelapp.test",
            FullName = "Verified User",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
        var handler = new CapturingHttpMessageHandler(_ => JsonResponse(HttpStatusCode.OK, response));
        var controller = CreateAuthController(handler, out var session);

        var result = await controller.VerifyEmail("email-token+value");

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<VerifyEmailViewModel>(view.Model);
        Assert.True(model.IsSuccess);
        Assert.Contains("auth/verify-email", handler.LastRequest?.RequestUri?.PathAndQuery ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("email-token%2Bvalue", handler.LastRequest?.RequestUri?.PathAndQuery ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("***", handler.LastRequest?.RequestUri?.PathAndQuery ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("verify-jwt-token", session.GetString("JWToken"));

        var logoutResult = controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(logoutResult);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
        Assert.Null(session.GetString("JWToken"));
        Assert.Null(session.GetString("UserEmail"));
        Assert.Null(session.GetString("UserFullName"));
    }

    // S5-02 için navbar'ın login state'e göre profil/logout veya login/register linklerini koruduğunu kontrol eder.
    [Fact]
    public void S502_LoginPartial_ShouldKeepNavbarAuthStateAndSafeLogoutForm()
    {
        // Auth hedefleri yoksa CI passed geçer; auth altyapısı varsa contract aktif olur.
        if (!Sprint5CardGuards.S502_AuthCrossTestTargetsImplemented())
            return;

        var loginPartial = ReadWebFile("Views", "Shared", "_LoginPartial.cshtml");

        AssertContainsAll(loginPartial, "IsAuthenticated", "UserFullName");
        AssertContainsAny(loginPartial, "asp-controller=\"Profile\"", "Profile/Index", "Profilim");
        AssertContainsAny(loginPartial, "asp-controller=\"Auth\"", "Auth/Logout", "Logout");
        AssertContainsAll(loginPartial, "method=\"post\"", "AntiForgeryToken");
        AssertContainsAny(loginPartial, "asp-action=\"Login\"", "Auth/Login", "Giriş Yap", "Giris Yap");
        AssertContainsAny(loginPartial, "asp-action=\"Register\"", "Auth/Register", "Kayıt Ol", "Kayit Ol");
        Assert.DoesNotContain("JWToken", loginPartial, StringComparison.OrdinalIgnoreCase);
    }

    // S5-02 için Profile, Reservations/My-Detail ve Reviews/My hedeflerinin başladığında auth guard kullandığını kontrol eder.
    [Fact]
    public void S502_ProtectedUserPages_ShouldUseAuthGuardAndPlannedApiEndpointsWhenStarted()
    {
        // Auth hedefleri yoksa CI passed geçer; auth altyapısı varsa contract aktif olur.
        if (!Sprint5CardGuards.S502_AuthCrossTestTargetsImplemented())
            return;

        var profileSource = ReadWebFile("Controllers", "ProfileController.cs");
        AssertContainsAny(profileSource, "IsAuthenticated", "AuthFilter", "Authorize");
        AssertContainsAll(profileSource, "auth/profile", "auth/change-password");
        AssertContainsAny(profileSource, "RedirectToAction(\"Login\"", "RedirectToAction(\"Login\", \"Auth\"");

        if (Sprint5CardGuards.S502_ReservationUserPagesImplemented())
        {
            var reservationsSource = ReadWebFile("Controllers", "ReservationsController.cs");
            AssertContainsAny(reservationsSource, "AuthFilter", "IsAuthenticated", "Authorize");
            AssertContainsAny(reservationsSource, "reservations/my", "reservations/", "PostAsync", "GetAsync");
        }

        if (Sprint5CardGuards.S502_ReviewsUserPagesImplemented())
        {
            var reviewsSource = ReadWebFile("Controllers", "ReviewsController.cs");
            AssertContainsAny(reviewsSource, "AuthFilter", "IsAuthenticated", "Authorize");
            AssertContainsAny(reviewsSource, "reviews/my", "reviews/", "DeleteAsync", "GetAsync");
        }
    }

    // S5-02 için auth form/action kaynaklarında register, login, verify, resend, profile update ve change password akışlarını kontrol eder.
    [Fact]
    public void S502_AuthFlowSources_ShouldKeepRegisterLoginVerifyResendProfileAndPasswordContracts()
    {
        // Auth hedefleri yoksa CI passed geçer; auth altyapısı varsa contract aktif olur.
        if (!Sprint5CardGuards.S502_AuthCrossTestTargetsImplemented())
            return;

        var authController = ReadWebFile("Controllers", "AuthController.cs");
        var loginView = ReadWebFile("Views", "Auth", "Login.cshtml");
        var registerView = ReadWebFile("Views", "Auth", "Register.cshtml");
        var verifyView = ReadWebFile("Views", "Auth", "VerifyEmail.cshtml");
        var apiService = ReadWebFile("Services", "ApiService.cs");

        AssertContainsAll(authController, "auth/login", "auth/register", "auth/verify-email", "auth/resend-verification", "Logout");
        AssertContainsAll(authController, "JWToken", "UserEmail", "UserFullName", "Session.Clear");
        AssertContainsAll(loginView, "RememberMe", "ReturnUrl", "AntiForgeryToken");
        AssertContainsAll(registerView, "AntiForgeryToken", "Password", "ConfirmPassword");
        AssertContainsAny(verifyView, "ResendVerification", "Tekrar", "doğrulama", "dogrulama");
        AssertContainsAll(apiService, "Authorization", "Bearer", "SetAuthHeader", "StatusCode");
        AssertContainsAny(apiService, "GetErrorMessageAsync", "Message", "Istek basarisiz oldu");
    }

    // Filter testleri için MVC ActionExecutingContext nesnesi hazırlar.
    private static ActionExecutingContext CreateExecutingContext(TestSession session)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };

        return new ActionExecutingContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: null!);
    }

    // Testler için role claim'i içeren basit JWT üretir.
    private static string CreateJwtToken(DateTime expires, string role)
    {
        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "user@hotelapp.test"),
                new Claim(ClaimTypes.Role, role)
            },
            expires: expires);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ApiService testleri için gerçek HttpClient yerine yakalayıcı handler kullanır.
    private static ApiService CreateApiService(CapturingHttpMessageHandler handler, TestSession session)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };
        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.hotelapp.test/")
        };

        return new ApiService(httpClient, accessor);
    }

    // AuthController testleri için controller, session ve sahte API cevabı hazırlar.
    private static WebAuthController CreateAuthController(CapturingHttpMessageHandler handler, out TestSession session)
    {
        session = new TestSession();
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };
        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.hotelapp.test/")
        };
        var apiService = new ApiService(httpClient, accessor);

        return new WebAuthController(apiService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    // JSON içerikli sahte HTTP cevabı üretir.
    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, object body)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json")
        };
    }

    // Web projesindeki bir dosyayı okur.
    private static string ReadWebFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.Web") }.Concat(relativePathParts).ToArray());

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

    // HttpClient isteklerini yakalayıp test cevabı döndüren handler.
    private sealed class CapturingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public CapturingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(_responseFactory(request));
        }
    }
}
