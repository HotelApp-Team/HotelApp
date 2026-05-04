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

public class Sprint4BookingReservationsContractTests
{
    private static readonly Assembly WebAssembly = typeof(WebHomeController).Assembly;
    private static readonly Assembly ApiAssembly = typeof(ApiAuthController).Assembly;

    [Fact]
    public void S406_BookingController_ShouldExposeProtectedCreatePostForE23()
    {
        if (!Sprint4CardGuards.S406_BookingCreateImplemented())
            return;

        var controllerType = GetRequiredWebType("HotelApp.Web.Controllers.BookingController");
        var createAction = FindAction(controllerType, "Create");

        Assert.NotNull(createAction);
        Assert.True(typeof(Controller).IsAssignableFrom(controllerType));
        Assert.True(
            controllerType.GetConstructors().Any(ctor => ctor.GetParameters().Any(p => p.ParameterType == typeof(ApiService))),
            "BookingController API ile konusmak icin ApiService dependency'si almali.");

        Assert.True(HasAuthGuard(controllerType, createAction!),
            "BookingController.Create sadece giris yapmis kullaniciya acik olmali.");
        Assert.True(createAction!.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            "BookingController.Create [HttpPost] olmali.");
        Assert.True(createAction.GetCustomAttributes<ValidateAntiForgeryTokenAttribute>(inherit: false).Any(),
            "BookingController.Create form guvenligi icin [ValidateAntiForgeryToken] kullanmali.");

        Assert.True(createAction.GetParameters().Any(),
            "BookingController.Create rezervasyon form modelini parametre olarak almali.");
    }

    [Fact]
    public void S406_BookingCreate_ShouldPostReservationToE23AndHandleResult()
    {
        if (!Sprint4CardGuards.S406_BookingCreateImplemented())
            return;

        var source = ReadWebFile("Controllers", "BookingController.cs");
        var createBody = ExtractMethodBodyOrExpression(source, "Create");

        AssertContainsAny(createBody, "PostAsync", "PostTAsync");
        Assert.Contains("reservations", createBody, StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(createBody, "TempData", "ModelState.AddModelError", "ViewBag");
        AssertContainsAny(createBody, "RedirectToAction");
        AssertContainsAny(createBody, "Detail", "My", "Reservations");
    }

    [Fact]
    public void S406_BookingFormModel_ShouldCarryReservationCreateFieldsFromPlan()
    {
        if (!Sprint4CardGuards.S406_BookingCreateImplemented())
            return;

        var controllerType = GetRequiredWebType("HotelApp.Web.Controllers.BookingController");
        var createAction = FindAction(controllerType, "Create")
            ?? throw new InvalidOperationException("BookingController.Create bulunamadi.");
        var modelType = createAction.GetParameters().FirstOrDefault()?.ParameterType
            ?? throw new InvalidOperationException("BookingController.Create model parametresi bulunamadi.");

        AssertHasAnyProperty(modelType, "HotelRoomId", "RoomId");
        AssertHasAnyProperty(modelType, "HotelId");
        AssertHasAnyProperty(modelType, "CheckInDate", "CheckIn");
        AssertHasAnyProperty(modelType, "CheckOutDate", "CheckOut");
        AssertHasAnyProperty(modelType, "GuestCount", "Guests", "AdultCount");
        AssertHasAnyProperty(modelType, "PaymentMethod");
    }

    [Fact]
    public void S406_ReservationsController_ShouldExposeProtectedMyAndCancelActionsFromPlan()
    {
        if (!Sprint4CardGuards.S406_UserReservationsPageImplemented())
            return;

        var controllerType = GetRequiredWebType("HotelApp.Web.Controllers.ReservationsController");
        var myAction = FindAction(controllerType, "My");
        var cancelAction = FindAction(controllerType, "Cancel");

        Assert.NotNull(myAction);
        Assert.NotNull(cancelAction);
        Assert.True(HasAuthGuard(controllerType, myAction!),
            "ReservationsController.My sadece giris yapmis kullaniciya acik olmali.");
        Assert.True(HasAuthGuard(controllerType, cancelAction!),
            "ReservationsController.Cancel sadece giris yapmis kullaniciya acik olmali.");

        Assert.False(myAction!.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            "ReservationsController.My GET ile acilan liste sayfasi olmali.");
        Assert.True(cancelAction!.GetParameters().Any(parameter =>
                string.Equals(parameter.Name, "id", StringComparison.OrdinalIgnoreCase)
                && (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(int?))),
            "ReservationsController.Cancel iptal edilecek rezervasyon id'sini almali.");
        Assert.True(cancelAction.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            "ReservationsController.Cancel [HttpPost] olmali.");
        Assert.True(cancelAction.GetCustomAttributes<ValidateAntiForgeryTokenAttribute>(inherit: false).Any(),
            "ReservationsController.Cancel form guvenligi icin [ValidateAntiForgeryToken] kullanmali.");
    }

    [Fact]
    public void S406_ReservationsController_ShouldCallE24AndE26ApiEndpoints()
    {
        if (!Sprint4CardGuards.S406_UserReservationsPageImplemented())
            return;

        var source = ReadWebFile("Controllers", "ReservationsController.cs");
        var myBody = ExtractMethodBodyOrExpression(source, "My");
        var cancelBody = ExtractMethodBodyOrExpression(source, "Cancel");

        AssertContainsAny(myBody, "GetAsync", "GetTAsync");
        Assert.Contains("reservations/my", myBody, StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(myBody, "status", "sortBy", "page", "pageSize", "filter");
        AssertContainsAny(myBody, "View(", "return View");

        AssertContainsAny(cancelBody, "PutAsync");
        AssertContainsAny(cancelBody, "{id}/cancel", "/cancel", "$\"reservations/{id}/cancel\"", "$\"api/reservations/{id}/cancel\"");
        AssertContainsAny(cancelBody, "TempData", "ModelState.AddModelError", "ViewBag");
        AssertContainsAny(cancelBody, "RedirectToAction(\"My\"", "RedirectToAction(nameof(My)", "My");
    }

    [Fact]
    public void S406_ReservationsMyView_ShouldRenderListFilterAndCancelFlow()
    {
        if (!Sprint4CardGuards.S406_UserReservationsPageImplemented())
            return;

        var view = ReadWebFile("Views", "Reservations", "My.cshtml");

        AssertContainsAny(view, "Rezervasyonlarım", "Rezervasyonlarim", "My Reservations");
        AssertContainsAny(view, "foreach", "@for", "Model.");
        AssertContainsAny(view, "HotelName", "Otel");
        AssertContainsAny(view, "RoomTypeName", "Oda");
        AssertContainsAny(view, "CheckInDate", "CheckIn", "Giriş", "Giris");
        AssertContainsAny(view, "CheckOutDate", "CheckOut", "Çıkış", "Cikis");
        AssertContainsAny(view, "GuestCount", "Misafir");
        AssertContainsAny(view, "TotalPrice", "Tutar", "Fiyat");
        AssertContainsAny(view, "Status", "Durum");

        AssertContainsAny(view, "Confirmed", "Cancelled", "Completed", "status");
        AssertContainsAny(view, "asp-action=\"Cancel\"", "Cancel", "İptal", "Iptal");
        AssertContainsAny(view, "asp-route-id", "ReservationId", "Model.Id", ".Id");
        AssertContainsAny(view, "method=\"post\"", "method=\"POST\"");
        AssertContainsAny(view, "AntiForgeryToken", "@Html.AntiForgeryToken", "asp-antiforgery");
    }

    [Fact]
    public void S406_LoginPartial_ShouldExposeReservationsMyLinkForLoggedInUsers()
    {
        if (!Sprint4CardGuards.S406_UserReservationsPageImplemented())
            return;

        var loginPartial = ReadWebFile("Views", "Shared", "_LoginPartial.cshtml");

        Assert.True(
            loginPartial.Contains("asp-controller=\"Reservations\"", StringComparison.OrdinalIgnoreCase)
                || loginPartial.Contains("href=\"/Reservations/My", StringComparison.OrdinalIgnoreCase)
                || loginPartial.Contains("Reservations/My", StringComparison.OrdinalIgnoreCase),
            "_LoginPartial giris yapan kullaniciya Rezervasyonlarim linki vermeli.");

        AssertContainsAny(loginPartial, "asp-action=\"My\"", "Reservations/My", "Rezervasyonlarım", "Rezervasyonlarim");
    }

    [Fact]
    public void S406_ApiReservations_ShouldExposeE23E24AndE26WhenUiStarts()
    {
        if (!Sprint4CardGuards.S406_BookingCreateImplemented()
            && !Sprint4CardGuards.S406_UserReservationsPageImplemented())
            return;

        var controllerType = ApiAssembly.GetTypes().FirstOrDefault(type =>
                string.Equals(type.FullName, "HotelApp.API.Controllers.ReservationController", StringComparison.Ordinal)
                || string.Equals(type.FullName, "HotelApp.API.Controllers.ReservationsController", StringComparison.Ordinal))
            ?? throw new InvalidOperationException("API Reservation(s)Controller tipi bulunamadi.");

        Assert.True(HasAuthorize(controllerType, method: null),
            "E23/E24/E26 rezervasyon endpoint'leri [Authorize] ile korunmali.");
        Assert.True(ControllerRouteContains(controllerType, "api/reservations"),
            "Reservation(s)Controller route'u api/reservations olmali.");

        var createAction = FindActionWithHttpAttribute<HttpPostAttribute>(controllerType, string.Empty);
        var myAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "my");
        var cancelAction = FindActionWithHttpAttribute<HttpPutAttribute>(controllerType, "{id}/cancel");

        Assert.NotNull(createAction.Method);
        Assert.NotNull(myAction.Method);
        Assert.NotNull(cancelAction.Method);
        Assert.True(HasAuthorize(controllerType, createAction.Method), "E23 POST /api/reservations [Authorize] ile korunmali.");
        Assert.True(HasAuthorize(controllerType, myAction.Method), "E24 GET /api/reservations/my [Authorize] ile korunmali.");
        Assert.True(HasAuthorize(controllerType, cancelAction.Method), "E26 PUT /api/reservations/{id}/cancel [Authorize] ile korunmali.");
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
            .Any(route => string.Equals(NormalizeRoute(route.Template), NormalizeRoute(expectedRoute), StringComparison.OrdinalIgnoreCase));
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
        return (template ?? string.Empty)
            .Trim()
            .Trim('/')
            .Replace("//", "/", StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);
    }

    private static void AssertHasAnyProperty(Type type, params string[] propertyNames)
    {
        Assert.True(
            propertyNames.Any(name => type.GetProperty(name) is not null),
            $"{type.Name} icinde beklenen alanlardan en az biri bulunmali: {string.Join(", ", propertyNames)}");
    }

    private static string ExtractMethodBodyOrExpression(string source, string methodName)
    {
        var match = Regex.Match(
            source,
            $@"\b(?:public|private|internal|protected)\s+(?:async\s+)?(?:Task\s*<[^>]+>|IActionResult|ActionResult|ViewResult)\s+{Regex.Escape(methodName)}\s*\([^)]*\)\s*(?<start>\{{|=>)",
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
