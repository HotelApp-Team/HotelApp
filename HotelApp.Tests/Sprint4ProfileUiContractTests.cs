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

public class Sprint4ProfileUiContractTests
{
    private static readonly Assembly WebAssembly = typeof(WebHomeController).Assembly;
    private static readonly Assembly ApiAssembly = typeof(ApiAuthController).Assembly;

    [Fact]
    public void S402_ProfileController_ShouldExposeProtectedProfileActionsFromPlan()
    {
        if (!Sprint4CardGuards.S402_ProfileUiImplemented())
            return;

        var controllerType = GetRequiredType(WebAssembly, "HotelApp.Web.Controllers.ProfileController");

        Assert.True(typeof(Controller).IsAssignableFrom(controllerType));
        Assert.True(
            controllerType.GetConstructors().Any(ctor => ctor.GetParameters().Any(p => p.ParameterType == typeof(ApiService))),
            "ProfileController API ile konusmak icin ApiService dependency'si almali.");

        var indexAction = FindAction(controllerType, "Index");
        var updateAction = FindAction(controllerType, "Update", "UpdateProfile");
        var changePasswordAction = FindAction(controllerType, "ChangePassword");

        Assert.NotNull(indexAction);
        Assert.NotNull(updateAction);
        Assert.NotNull(changePasswordAction);

        Assert.True(HasAuthGuard(controllerType, indexAction!),
            "ProfileController.Index session/token yoksa Login'e yonlendirecek AuthFilter/Authorize/IsAuthenticated korumasina sahip olmali.");

        AssertPostFormAction(updateAction!, "profil guncelleme");
        AssertPostFormAction(changePasswordAction!, "sifre degistirme");
    }

    [Fact]
    public void S402_ProfileController_ShouldCallE03E46AndE47ApiEndpoints()
    {
        if (!Sprint4CardGuards.S402_ProfileUiImplemented())
            return;

        var source = ReadWebFile("Controllers", "ProfileController.cs");

        AssertContainsAny(source, "GetAsync", "GetTAsync");
        AssertContainsAny(source, "PutAsync");
        Assert.Contains("auth/profile", source, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("auth/change-password", source, StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(source, "ModelState.AddModelError", "TempData", "ViewBag");
        AssertContainsAny(source, "RedirectToAction(\"Login\"", "AuthFilter", "IsAuthenticated");
    }

    [Fact]
    public void S402_ProfileView_ShouldRenderProfileAndPasswordFormsFromPlan()
    {
        if (!Sprint4CardGuards.S402_ProfileUiImplemented())
            return;

        var view = ReadWebFile("Views", "Profile", "Index.cshtml");
        var formCount = Regex.Matches(view, "<form", RegexOptions.IgnoreCase).Count;

        Assert.True(formCount >= 2, "Profile/Index.cshtml profil ve sifre degistirme icin iki ayri form icermeli.");
        AssertContainsAny(view, "asp-action=\"Update\"", "asp-action=\"UpdateProfile\"", "UpdateProfile");
        AssertContainsAny(view, "asp-action=\"ChangePassword\"", "ChangePassword");

        AssertContainsAny(view, "FirstName", "Ad");
        AssertContainsAny(view, "LastName", "Soyad");
        AssertContainsAny(view, "Phone", "Telefon");
        AssertContainsAny(view, "Email", "E-posta", "Eposta");

        AssertContainsAny(view, "CurrentPassword", "Mevcut");
        AssertContainsAny(view, "NewPassword", "Yeni");
        AssertContainsAny(view, "ConfirmNewPassword", "Tekrar");

        AssertContainsAny(view, "asp-validation-summary", "ValidationSummary", "asp-validation-for");
        AssertContainsAny(view, "TempData", "SuccessMessage", "ErrorMessage", "alert");
    }

    [Fact]
    public void S402_LoginPartial_ShouldExposeProfileLinkForLoggedInUsers()
    {
        if (!Sprint4CardGuards.S402_ProfileUiImplemented())
            return;

        var loginPartial = ReadWebFile("Views", "Shared", "_LoginPartial.cshtml");

        AssertContainsAny(loginPartial, "asp-controller=\"Profile\"", "href=\"/Profile", "Profile/Index");
        AssertContainsAny(loginPartial, "asp-action=\"Index\"", "/Profile", "Profil");
    }

    [Fact]
    public void S402_AuthApi_ShouldExposeProfileUpdateAndChangePasswordEndpointsForUi()
    {
        if (!Sprint4CardGuards.S402_ProfileUiImplemented())
            return;

        var authControllerType = ApiAssembly.GetType("HotelApp.API.Controllers.AuthController")
            ?? throw new InvalidOperationException("HotelApp.API.Controllers.AuthController tipi bulunamadi.");

        var updateProfileAction = FindActionWithHttpAttribute<HttpPutAttribute>(authControllerType, "profile");
        var changePasswordAction = FindActionWithHttpAttribute<HttpPutAttribute>(authControllerType, "change-password");

        Assert.NotNull(updateProfileAction.Method);
        Assert.NotNull(changePasswordAction.Method);
        Assert.True(HasAuthorize(authControllerType, updateProfileAction.Method!),
            "E46 PUT /api/auth/profile [Authorize] ile korunmali.");
        Assert.True(HasAuthorize(authControllerType, changePasswordAction.Method!),
            "E47 PUT /api/auth/change-password [Authorize] ile korunmali.");
    }

    private static void AssertPostFormAction(MethodInfo method, string actionDescription)
    {
        Assert.True(
            method.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            $"{actionDescription} action'i [HttpPost] olmali.");

        Assert.True(
            method.GetCustomAttributes<ValidateAntiForgeryTokenAttribute>(inherit: false).Any(),
            $"{actionDescription} action'i form guvenligi icin [ValidateAntiForgeryToken] kullanmali.");
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

        var source = ReadWebFile("Controllers", "ProfileController.cs");
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

    private static bool HasAuthorize(Type controllerType, MethodInfo method)
    {
        return controllerType.GetCustomAttribute<AuthorizeAttribute>(inherit: true) is not null
            || method.GetCustomAttribute<AuthorizeAttribute>(inherit: true) is not null;
    }

    private static MethodInfo? FindAction(Type controllerType, params string[] actionNames)
    {
        return controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .FirstOrDefault(method => actionNames.Any(name => string.Equals(method.Name, name, StringComparison.OrdinalIgnoreCase)));
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

    private static Type GetRequiredType(Assembly assembly, string fullName)
    {
        return assembly.GetType(fullName)
            ?? throw new InvalidOperationException($"{fullName} tipi bulunamadi.");
    }

    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase)),
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
