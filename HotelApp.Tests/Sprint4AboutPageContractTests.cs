using WebHomeController = HotelApp.Web.Controllers.HomeController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HotelApp.Tests;

public class Sprint4AboutPageContractTests
{
    private static readonly Assembly WebAssembly = typeof(WebHomeController).Assembly;

    [Fact]
    public void S403_HomeController_ShouldExposePublicAboutActionFromPlan()
    {
        if (!Sprint4CardGuards.S403_AboutPageImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.Web.Controllers.HomeController");
        var aboutAction = FindAction(controllerType, "About");

        Assert.NotNull(aboutAction);
        Assert.True(
            IsActionResultType(aboutAction!.ReturnType),
            "HomeController.About action'i IActionResult, ViewResult veya Task<IActionResult> donmeli.");

        Assert.False(
            HasAuthorize(controllerType, aboutAction),
            "S4-03 About sayfasi public/statik sayfa oldugu icin [Authorize] ile korunmamali.");

        Assert.False(
            aboutAction.GetCustomAttributes<HttpPostAttribute>(inherit: false).Any(),
            "S4-03 About sayfasi GET ile acilan statik sayfa olmali; [HttpPost] olmamali.");
    }

    [Fact]
    public void S403_AboutAction_ShouldBeStaticAndReturnViewWithoutApiCall()
    {
        if (!Sprint4CardGuards.S403_AboutPageImplemented())
            return;

        var source = ReadWebFile("Controllers", "HomeController.cs");
        var aboutBody = ExtractMethodBodyOrExpression(source, "About");

        AssertContainsAny(aboutBody, "View(", "return View");
        Assert.DoesNotContain("_apiService", aboutBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GetAsync", aboutBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PostAsync", aboutBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PutAsync", aboutBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DeleteAsync", aboutBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void S403_AboutView_ShouldContainStaticTeamAndProjectContentFromPlan()
    {
        if (!Sprint4CardGuards.S403_AboutPageImplemented())
            return;

        var view = ReadWebFile("Views", "Home", "About.cshtml");

        AssertContainsAny(view, "Hakkımızda", "Hakkimizda", "About");
        AssertContainsAny(view, "HotelApp", "otel", "rezervasyon", "konaklama");
        AssertContainsAny(view, "takım", "takim", "ekip", "Alper", "Sevde", "Hilal", "Grup");
        AssertContainsAny(view, "ViewData[\"Title\"]", "ViewBag.Title", "<h1", "<h2");

        Assert.DoesNotContain("<form", view, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("fetch(", view, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("$.ajax", view, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void S403_PublicLayout_ShouldLinkToAboutPage()
    {
        if (!Sprint4CardGuards.S403_AboutPageImplemented())
            return;

        var layout = ReadWebFile("Views", "Shared", "_Layout.cshtml");

        Assert.True(
            layout.Contains("asp-action=\"About\"", StringComparison.OrdinalIgnoreCase)
                || layout.Contains("href=\"/Home/About", StringComparison.OrdinalIgnoreCase)
                || layout.Contains("href=\"/About", StringComparison.OrdinalIgnoreCase),
            "_Layout icinde About/Hakkimizda sayfasina navbar linki bulunmali.");

        AssertContainsAny(layout, "Hakkımızda", "Hakkimizda", "About");
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
