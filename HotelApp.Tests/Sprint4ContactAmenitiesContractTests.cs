using WebHomeController = HotelApp.Web.Controllers.HomeController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HotelApp.Tests;

public class Sprint4ContactAmenitiesContractTests
{
    private static readonly Assembly WebAssembly = typeof(WebHomeController).Assembly;

    [Fact]
    public void S404_HomeController_ShouldExposePublicStaticContactActionFromPlan()
    {
        if (!Sprint4CardGuards.S404_ContactPageImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.Web.Controllers.HomeController");
        var contactAction = FindAction(controllerType, "Contact");

        Assert.NotNull(contactAction);
        Assert.True(
            IsActionResultType(contactAction!.ReturnType),
            "HomeController.Contact action'i IActionResult, ViewResult veya Task<IActionResult> donmeli.");

        Assert.False(
            HasAuthorize(controllerType, contactAction),
            "S4-04 Contact sayfasi public/statik sayfa oldugu icin [Authorize] ile korunmamali.");

        Assert.False(
            contactAction.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            "S4-04 Contact sayfasi GET ile acilan statik sayfa olmali; [HttpPost] olmamali.");
    }

    [Fact]
    public void S404_ContactAction_ShouldReturnViewWithoutApiCall()
    {
        if (!Sprint4CardGuards.S404_ContactPageImplemented())
            return;

        var source = ReadWebFile("Controllers", "HomeController.cs");
        var contactBody = ExtractMethodBodyOrExpression(source, "Contact");

        AssertContainsAny(contactBody, "View(", "return View");
        Assert.DoesNotContain("_apiService", contactBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GetAsync", contactBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PostAsync", contactBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PutAsync", contactBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DeleteAsync", contactBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void S404_ContactView_ShouldContainStaticContactInformationFromPlan()
    {
        if (!Sprint4CardGuards.S404_ContactPageImplemented())
            return;

        var view = ReadWebFile("Views", "Home", "Contact.cshtml");

        AssertContainsAny(view, "İletişim", "Iletisim", "Contact");
        AssertContainsAny(view, "ViewData[\"Title\"]", "ViewBag.Title", "<h1", "<h2");

        Assert.True(
            ContainsAny(view, "@", "mail", "email", "e-posta", "telefon", "phone", "adres", "address", "harita", "map", "<form"),
            "Contact.cshtml iletisim bilgileri veya statik bir iletisim formu icermeli.");

        Assert.DoesNotContain("fetch(", view, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("$.ajax", view, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("asp-action=\"Contact\" method=\"post\"", view, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void S404_PublicLayout_ShouldLinkToContactPage()
    {
        if (!Sprint4CardGuards.S404_ContactPageImplemented())
            return;

        var layout = ReadWebFile("Views", "Shared", "_Layout.cshtml");

        Assert.True(
            layout.Contains("asp-action=\"Contact\"", StringComparison.OrdinalIgnoreCase)
                || layout.Contains("href=\"/Home/Contact", StringComparison.OrdinalIgnoreCase)
                || layout.Contains("href=\"/Contact", StringComparison.OrdinalIgnoreCase),
            "_Layout icinde Contact/Iletisim sayfasina navbar linki bulunmali.");

        AssertContainsAny(layout, "İletişim", "Iletisim", "Contact");
    }

    [Fact]
    public void S404_HotelDetailPageVm_ShouldExposeAmenitiesWithNameAndIconContract()
    {
        if (!Sprint4CardGuards.S404_HotelDetailAmenitiesImplemented())
            return;

        var vmType = GetRequiredType("HotelApp.Web.Models.HotelDetailPageVm");
        var amenitiesProperty = GetAnyProperty(vmType, "Amenities", "HotelAmenities", "Facilities");

        Assert.NotNull(amenitiesProperty);
        Assert.True(
            IsEnumerableButNotString(amenitiesProperty!.PropertyType),
            "HotelDetailPageVm olanaklari liste olarak tasimali.");

        var itemType = GetEnumerableItemType(amenitiesProperty.PropertyType)
            ?? throw new InvalidOperationException("Amenities liste eleman tipi okunamadi.");

        AssertHasAnyProperty(itemType, "Name", "Title");
        AssertHasAnyProperty(itemType, "Icon", "IconClass", "CssIcon");
    }

    [Fact]
    public void S404_HotelsControllerDetail_ShouldUseMasterViewModelForAmenitiesIntegration()
    {
        if (!Sprint4CardGuards.S404_HotelDetailAmenitiesImplemented())
            return;

        var source = ReadWebFile("Controllers", "HotelsController.cs");
        var detailBody = ExtractMethodBodyOrExpression(source, "Detail");

        AssertContainsAny(detailBody, "HotelDetailPageVm", "HotelDetailViewModel");
        AssertContainsAny(detailBody, "GetAsync", "ApiService");
        AssertContainsAny(detailBody, "hotels/{id}", "Hotels/{id}", "$\"Hotels/{id}\"", "$\"hotels/{id}\"");
        AssertContainsAny(detailBody, "return View", "View(\"Detail\"", "View(model)");
    }

    [Fact]
    public void S404_DetailView_ShouldRenderAmenitiesSectionFromPlan()
    {
        if (!Sprint4CardGuards.S404_HotelDetailAmenitiesImplemented())
            return;

        var detailView = ReadWebFile("Views", "Hotels", "Detail.cshtml");

        AssertContainsAny(detailView, "Olanak", "Özellik", "Ozellik", "Amenit", "Facilities");
        AssertContainsAny(detailView, "Model.Amenities", "Model.HotelAmenities", "Model.Facilities");
        AssertContainsAny(detailView, "foreach", "for (", "@for");
        AssertContainsAny(detailView, ".Name", "Name", ".Title", "Title");
        AssertContainsAny(detailView, "amenity", "facility", "olanak", "özellik", "ozellik");
    }

    private static Type GetRequiredType(string fullName)
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

    private static bool IsActionResultType(Type returnType)
    {
        if (typeof(IActionResult).IsAssignableFrom(returnType))
            return true;

        if (typeof(ActionResult).IsAssignableFrom(returnType))
            return true;

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            return IsActionResultType(returnType.GetGenericArguments()[0]);

        return false;
    }

    private static bool HasAuthorize(Type controllerType, MethodInfo method)
    {
        return controllerType.GetCustomAttribute<AuthorizeAttribute>(inherit: true) is not null
            || method.GetCustomAttribute<AuthorizeAttribute>(inherit: true) is not null;
    }

    private static PropertyInfo? GetAnyProperty(Type type, params string[] propertyNames)
    {
        return propertyNames
            .Select(name => type.GetProperty(name))
            .FirstOrDefault(property => property is not null);
    }

    private static void AssertHasAnyProperty(Type type, params string[] propertyNames)
    {
        Assert.True(
            propertyNames.Any(name => type.GetProperty(name) is not null),
            $"{type.Name} icinde beklenen alanlardan en az biri bulunmali: {string.Join(", ", propertyNames)}");
    }

    private static bool IsEnumerableButNotString(Type type)
    {
        return type != typeof(string)
            && type.GetInterfaces().Append(type).Any(interfaceType =>
                interfaceType.IsGenericType
                && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    private static Type? GetEnumerableItemType(Type type)
    {
        return type.GetInterfaces()
            .Append(type)
            .Where(interfaceType => interfaceType.IsGenericType)
            .FirstOrDefault(interfaceType => interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            ?.GetGenericArguments()[0];
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
