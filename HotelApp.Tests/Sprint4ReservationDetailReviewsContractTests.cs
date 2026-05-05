using ApiAuthController = HotelApp.API.Controllers.AuthController;
using WebHomeController = HotelApp.Web.Controllers.HomeController;
using HotelApp.Web.Filters;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HotelApp.Tests;

// Sprint4ReservationDetailReviewsContractTests sınıfının amacı, S4-07 kartındaki rezervasyon detay ve yorumlarım UI sözleşmesini korumaktır.
// Kod yokken CI passed geçer; kod başladığında Trello ve plan kapsamındaki controller, view ve API bağlantılarını doğrular.
// SOLID: SRP kullanır; sadece S4-07 frontend/API contract kontrollerinden sorumludur.
public class Sprint4ReservationDetailReviewsContractTests
{
    private static readonly Assembly WebAssembly = typeof(WebHomeController).Assembly;
    private static readonly Assembly ApiAssembly = typeof(ApiAuthController).Assembly;

    // S4-07 için ReservationsController.Detail action'ının güvenli ve plana uygun açıldığını kontrol eder.
    [Fact]
    public void S407_ReservationsController_ShouldExposeProtectedDetailActionForE25()
    {
        // S4-07 detay kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_ReservationDetailImplemented())
            return;

        var controllerType = GetRequiredWebType("HotelApp.Web.Controllers.ReservationsController");
        var detailAction = FindAction(controllerType, "Detail");

        Assert.NotNull(detailAction);
        Assert.True(typeof(Controller).IsAssignableFrom(controllerType));
        Assert.True(
            controllerType.GetConstructors().Any(ctor => ctor.GetParameters().Any(p => p.ParameterType == typeof(ApiService))),
            "ReservationsController API ile konusmak icin ApiService dependency'si almali.");

        Assert.True(HasAuthGuard(controllerType, detailAction!),
            "ReservationsController.Detail sadece giris yapmis kullaniciya acik olmali.");
        Assert.False(detailAction!.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            "ReservationsController.Detail GET ile acilan detay sayfasi olmali.");
        AssertHasIdParameter(detailAction, "id", "reservationId");
    }

    // S4-07 için rezervasyon detay sayfasının E25 endpointinden veri aldığını kontrol eder.
    [Fact]
    public void S407_ReservationsController_ShouldCallE25EndpointAndReturnDetailView()
    {
        // S4-07 detay kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_ReservationDetailImplemented())
            return;

        var source = ReadWebFile("Controllers", "ReservationsController.cs");
        var detailBody = ExtractMethodBodyOrExpression(source, "Detail");

        AssertContainsAny(detailBody, "GetAsync", "GetTAsync");
        Assert.True(
            detailBody.Contains("reservations/", StringComparison.OrdinalIgnoreCase)
                && !detailBody.Contains("reservations/my", StringComparison.OrdinalIgnoreCase)
                && !detailBody.Contains("/cancel", StringComparison.OrdinalIgnoreCase),
            "ReservationsController.Detail E25 icin /api/reservations/{id} endpointini cagirmali.");
        AssertContainsAny(detailBody, "View(", "return View");
        AssertContainsAny(detailBody, "TempData", "ModelState.AddModelError", "ViewBag", "NotFound", "RedirectToAction");
    }

    // S4-07 için Reservations/Detail view'inin detay alanlarını ve fatura akışını gösterdiğini kontrol eder.
    [Fact]
    public void S407_ReservationsDetailView_ShouldRenderReservationFieldsAndInvoiceFlow()
    {
        // S4-07 detay kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_ReservationDetailImplemented())
            return;

        var view = ReadWebFile("Views", "Reservations", "Detail.cshtml");

        AssertContainsAny(view, "Rezervasyon Detay", "Rezervasyon Detayı", "Reservation Detail");
        AssertContainsAny(view, "HotelName", "Otel");
        AssertContainsAny(view, "RoomTypeName", "RoomType", "Oda");
        AssertContainsAny(view, "CheckInDate", "CheckIn", "Giriş", "Giris");
        AssertContainsAny(view, "CheckOutDate", "CheckOut", "Çıkış", "Cikis");
        AssertContainsAny(view, "GuestCount", "Misafir");
        AssertContainsAny(view, "TotalPrice", "Tutar", "Fiyat");
        AssertContainsAny(view, "Status", "Durum");
        Assert.True(
            ContainsAny(view, "asp-action=\"Invoice\"", "asp-action=\"GetInvoice\"", "DownloadInvoice", "/invoice", "api/reservations")
                || (ContainsAny(view, "Fatura", "fatura", "Invoice", "invoice") && view.Contains("href", StringComparison.OrdinalIgnoreCase)),
            "Reservations/Detail.cshtml E27 fatura goruntuleme/indirme akisini link veya butonla gostermeli.");
    }

    // S4-07 için ReviewsController.My ve Delete action'larının güvenli çalıştığını kontrol eder.
    [Fact]
    public void S407_ReviewsController_ShouldExposeProtectedMyAndDeleteActionsFromPlan()
    {
        // S4-07 yorumlarım kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_MyReviewsImplemented())
            return;

        var controllerType = GetRequiredWebType("HotelApp.Web.Controllers.ReviewsController");
        var myAction = FindAction(controllerType, "My");
        var deleteAction = FindAction(controllerType, "Delete");

        Assert.NotNull(myAction);
        Assert.NotNull(deleteAction);
        Assert.True(
            controllerType.GetConstructors().Any(ctor => ctor.GetParameters().Any(p => p.ParameterType == typeof(ApiService))),
            "ReviewsController API ile konusmak icin ApiService dependency'si almali.");

        Assert.True(HasAuthGuard(controllerType, myAction!),
            "ReviewsController.My sadece giris yapmis kullaniciya acik olmali.");
        Assert.True(HasAuthGuard(controllerType, deleteAction!),
            "ReviewsController.Delete sadece giris yapmis kullaniciya acik olmali.");
        Assert.False(myAction!.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            "ReviewsController.My GET ile acilan yorumlarim sayfasi olmali.");
        AssertHasIdParameter(deleteAction!, "id", "reviewId");
        Assert.True(deleteAction!.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            "ReviewsController.Delete UI silme islemi icin [HttpPost] olmali.");
        Assert.True(deleteAction.GetCustomAttributes<ValidateAntiForgeryTokenAttribute>(inherit: false).Any(),
            "ReviewsController.Delete form guvenligi icin [ValidateAntiForgeryToken] kullanmali.");
    }

    // S4-07 için ReviewsController'ın E31 ve E30 endpointlerini çağırdığını kontrol eder.
    [Fact]
    public void S407_ReviewsController_ShouldCallE31AndE30ApiEndpoints()
    {
        // S4-07 yorumlarım kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_MyReviewsImplemented())
            return;

        var source = ReadWebFile("Controllers", "ReviewsController.cs");
        var myBody = ExtractMethodBodyOrExpression(source, "My");
        var deleteBody = ExtractMethodBodyOrExpression(source, "Delete");

        AssertContainsAny(myBody, "GetAsync", "GetTAsync");
        Assert.Contains("reviews/my", myBody, StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(myBody, "View(", "return View");

        AssertContainsAny(deleteBody, "DeleteAsync", "SendAsync");
        Assert.True(
            deleteBody.Contains("reviews/", StringComparison.OrdinalIgnoreCase)
                && !deleteBody.Contains("reviews/my", StringComparison.OrdinalIgnoreCase),
            "ReviewsController.Delete E30 icin /api/reviews/{id} endpointini cagirmali.");
        AssertContainsAny(deleteBody, "TempData", "ModelState.AddModelError", "ViewBag");
        AssertContainsAny(deleteBody, "RedirectToAction(\"My\"", "RedirectToAction(nameof(My)", "My");
    }

    // S4-07 için Reviews/My view'inin yorum listesini ve silme formunu gösterdiğini kontrol eder.
    [Fact]
    public void S407_ReviewsMyView_ShouldRenderOwnReviewsAndDeleteFlow()
    {
        // S4-07 yorumlarım kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_MyReviewsImplemented())
            return;

        var view = ReadWebFile("Views", "Reviews", "My.cshtml");

        AssertContainsAny(view, "Yorumlarım", "Yorumlarim", "My Reviews");
        AssertContainsAny(view, "foreach", "@for", "Model.");
        AssertContainsAny(view, "HotelName", "Otel");
        AssertContainsAny(view, "Rating", "Puan", "Yıldız", "Yildiz");
        AssertContainsAny(view, "Comment", "ReviewText", "Yorum");
        AssertContainsAny(view, "CreatedAt", "Date", "Tarih");
        AssertContainsAny(view, "asp-action=\"Delete\"", "Delete", "Sil");
        AssertContainsAny(view, "asp-route-id", "ReviewId", "Model.Id", ".Id");
        AssertContainsAny(view, "method=\"post\"", "method=\"POST\"");
        AssertContainsAny(view, "AntiForgeryToken", "@Html.AntiForgeryToken", "asp-antiforgery");
    }

    // S4-07 için giriş yapan kullanıcı menüsünde Yorumlarım linkinin göründüğünü kontrol eder.
    [Fact]
    public void S407_LoginPartial_ShouldExposeReviewsMyLinkForLoggedInUsers()
    {
        // S4-07 yorumlarım kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_MyReviewsImplemented())
            return;

        var loginPartial = ReadWebFile("Views", "Shared", "_LoginPartial.cshtml");

        Assert.True(
            loginPartial.Contains("Reviews/My", StringComparison.OrdinalIgnoreCase)
                || (loginPartial.Contains("asp-controller=\"Reviews\"", StringComparison.OrdinalIgnoreCase)
                    && loginPartial.Contains("asp-action=\"My\"", StringComparison.OrdinalIgnoreCase)),
            "_LoginPartial giris yapan kullaniciya Yorumlarim linki vermeli.");
        AssertContainsAny(loginPartial, "Yorumlarım", "Yorumlarim", "My Reviews", "asp-action=\"My\"", "Reviews/My");
    }

    // S4-07 detay UI başladıysa API tarafındaki E25 ve E27 route'larını kontrol eder.
    [Fact]
    public void S407_ApiReservations_ShouldExposeE25AndE27WhenDetailUiStarts()
    {
        // S4-07 detay kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_ReservationDetailImplemented())
            return;

        var controllerType = ApiAssembly.GetTypes().FirstOrDefault(type =>
                string.Equals(type.FullName, "HotelApp.API.Controllers.ReservationController", StringComparison.Ordinal)
                || string.Equals(type.FullName, "HotelApp.API.Controllers.ReservationsController", StringComparison.Ordinal))
            ?? throw new InvalidOperationException("API Reservation(s)Controller tipi bulunamadi.");

        Assert.True(ControllerRouteContains(controllerType, "api/reservations"),
            "Reservation(s)Controller route'u api/reservations olmali.");

        var detailAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "{id}");
        var invoiceAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "{id}/invoice");

        Assert.NotNull(detailAction.Method);
        Assert.NotNull(invoiceAction.Method);
        Assert.True(HasAuthorize(controllerType, detailAction.Method), "E25 GET /api/reservations/{id} [Authorize] ile korunmali.");
        Assert.True(HasAuthorize(controllerType, invoiceAction.Method), "E27 GET /api/reservations/{id}/invoice [Authorize] ile korunmali.");
    }

    // S4-07 yorumlarım UI başladıysa API tarafındaki E31 ve E30 route'larını kontrol eder.
    [Fact]
    public void S407_ApiReviews_ShouldExposeE31AndE30WhenMyReviewsUiStarts()
    {
        // S4-07 yorumlarım kodu yoksa CI passed geçer; kod başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S407_MyReviewsImplemented())
            return;

        var controllerType = ApiAssembly.GetTypes().FirstOrDefault(type =>
                string.Equals(type.FullName, "HotelApp.API.Controllers.ReviewController", StringComparison.Ordinal)
                || string.Equals(type.FullName, "HotelApp.API.Controllers.ReviewsController", StringComparison.Ordinal))
            ?? throw new InvalidOperationException("API Review(s)Controller tipi bulunamadi.");

        Assert.True(ControllerRouteContains(controllerType, "api/reviews"),
            "Review(s)Controller route'u api/reviews olmali.");

        var myAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "my");
        var deleteAction = FindActionWithHttpAttribute<HttpDeleteAttribute>(controllerType, "{id}");

        Assert.NotNull(myAction.Method);
        Assert.NotNull(deleteAction.Method);
        Assert.True(HasAuthorize(controllerType, myAction.Method), "E31 GET /api/reviews/my [Authorize] ile korunmali.");
        Assert.True(HasAuthorize(controllerType, deleteAction.Method), "E30 DELETE /api/reviews/{id} [Authorize] ile korunmali.");
    }

    private static bool HasAuthGuard(Type controllerType, MethodInfo method)
    {
        if (HasAuthorize(controllerType, method))
            return true;

        if (controllerType.GetCustomAttributes<AuthFilter>(inherit: true).Any()
            || method.GetCustomAttributes<AuthFilter>(inherit: true).Any())
            return true;

        if (HasServiceFilterForAuthFilter(controllerType.GetCustomAttributes(inherit: true))
            || HasServiceFilterForAuthFilter(method.GetCustomAttributes(inherit: true)))
            return true;

        var controllerPath = Path.Combine("Controllers", $"{controllerType.Name}.cs");
        var source = ReadWebFile(controllerPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        return source.Contains("AuthFilter", StringComparison.OrdinalIgnoreCase)
            || (source.Contains("IsAuthenticated", StringComparison.OrdinalIgnoreCase)
                && source.Contains("RedirectToAction", StringComparison.OrdinalIgnoreCase)
                && source.Contains("Login", StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasServiceFilterForAuthFilter(IEnumerable<object> attributes)
    {
        return attributes
            .OfType<ServiceFilterAttribute>()
            .Any(attribute => attribute.ServiceType == typeof(AuthFilter));
    }

    private static bool HasAuthorize(Type controllerType, MethodInfo? method)
    {
        return controllerType.GetCustomAttribute<AuthorizeAttribute>(inherit: true) is not null
            || method?.GetCustomAttribute<AuthorizeAttribute>(inherit: true) is not null;
    }

    private static bool ControllerRouteContains(Type controllerType, string expectedRoute)
    {
        return controllerType
            .GetCustomAttributes<RouteAttribute>(inherit: false)
            .Any(route => RouteEquals(ExpandControllerToken(route.Template, controllerType), expectedRoute));
    }

    private static Type GetRequiredWebType(string fullName)
    {
        return WebAssembly.GetType(fullName)
            ?? throw new InvalidOperationException($"{fullName} tipi bulunamadi.");
    }

    private static MethodInfo? FindAction(Type controllerType, string actionName)
    {
        return controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .FirstOrDefault(method => string.Equals(method.Name, actionName, StringComparison.OrdinalIgnoreCase));
    }

    private static (MethodInfo? Method, TAttribute? Attribute) FindActionWithHttpAttribute<TAttribute>(Type controllerType, string expectedTemplate)
        where TAttribute : HttpMethodAttribute
    {
        return controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .SelectMany(method => method
                .GetCustomAttributes<TAttribute>(inherit: false)
                .Select(attribute => (Method: (MethodInfo?)method, Attribute: (TAttribute?)attribute)))
            .FirstOrDefault(action => RouteEquals(action.Attribute?.Template, expectedTemplate));
    }

    private static bool RouteEquals(string? actualTemplate, string expectedTemplate)
    {
        return string.Equals(
            NormalizeRoute(actualTemplate),
            NormalizeRoute(expectedTemplate),
            StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeRoute(string? template)
    {
        var normalized = (template ?? string.Empty)
            .Trim()
            .Trim('/')
            .Replace("//", "/", StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        normalized = Regex.Replace(normalized, @"\{([^}:]+):[^}]+\}", "{$1}");
        normalized = Regex.Replace(normalized, @"\{reservationId\}", "{id}", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"\{reviewId\}", "{id}", RegexOptions.IgnoreCase);

        return normalized;
    }

    private static string ExpandControllerToken(string? template, Type controllerType)
    {
        var controllerName = controllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            ? controllerType.Name[..^"Controller".Length]
            : controllerType.Name;

        return (template ?? string.Empty).Replace("[controller]", controllerName, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertHasIdParameter(MethodInfo method, params string[] expectedNames)
    {
        Assert.True(
            method.GetParameters().Any(parameter =>
                expectedNames.Any(name => string.Equals(parameter.Name, name, StringComparison.OrdinalIgnoreCase))
                && (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(int?))),
            $"{method.Name} action'i id parametresi almali: {string.Join(", ", expectedNames)}");
    }

    private static string ExtractMethodBodyOrExpression(string source, string methodName)
    {
        var match = Regex.Match(
            source,
            $@"\b(?:public|private|internal|protected)\s+(?:async\s+)?(?:Task\s*<[^>]+>|IActionResult|ActionResult|ViewResult|FileResult)\s+{Regex.Escape(methodName)}\s*\([^)]*\)\s*(?<start>\{{|=>)",
            RegexOptions.Multiline);

        Assert.True(match.Success, $"{methodName} metodu bulunamadi.");

        if (match.Groups["start"].Value == "=>")
        {
            var semicolonIndex = source.IndexOf(';', match.Index);
            Assert.True(semicolonIndex >= 0, $"{methodName} expression-bodied metodu icin ; bulunamadi.");
            return source[match.Index..(semicolonIndex + 1)];
        }

        var openBraceIndex = source.IndexOf('{', match.Index);
        Assert.True(openBraceIndex >= 0, $"{methodName} metodu icin acilis blogu bulunamadi.");

        var depth = 0;
        for (var i = openBraceIndex; i < source.Length; i++)
        {
            if (source[i] == '{')
                depth++;
            else if (source[i] == '}')
                depth--;

            if (depth == 0)
                return source[openBraceIndex..(i + 1)];
        }

        throw new InvalidOperationException($"{methodName} metodu govdesi okunamadi.");
    }

    private static bool ContainsAny(string text, params string[] expectedParts)
    {
        return expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase));
    }

    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            ContainsAny(text, expectedParts),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }

    private static string ReadWebFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindWebProjectRoot() }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

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
