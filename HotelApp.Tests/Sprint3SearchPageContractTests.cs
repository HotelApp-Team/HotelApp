using HotelApp.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace HotelApp.Tests;

public class Sprint3SearchPageContractTests
{
    private static readonly Assembly WebAssembly = typeof(HomeController).Assembly;

    [Fact]
    public void HotelsController_ShouldExposePublicSearchActionAndCallE07SearchEndpoint()
    {
        if (!Sprint3CardGuards.S304_SearchPageImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.Web.Controllers.HotelsController");
        var searchAction = controllerType.GetMethod("Search", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        Assert.NotNull(searchAction);
        Assert.True(
            AllowsAnonymous(searchAction) || !RequiresAuthorization(controllerType, searchAction),
            "HotelsController.Search public sayfa olmali; auth zorunlulugu olmamali veya action [AllowAnonymous] olmali.");

        var source = ReadWebFile("Controllers", "HotelsController.cs");

        AssertContainsAny(source, "hotels/search", "api/hotels/search");
        AssertContainsAll(source, "city", "checkIn", "checkOut", "guests", "minStarRating", "page", "pageSize");
        AssertContainsAny(source, "ApiService", "GetAsync", "HttpClient");
    }

    [Fact]
    public void SearchView_ShouldRenderResultsFiltersSortingAndPaginationFromCard()
    {
        if (!Sprint3CardGuards.S304_SearchPageImplemented())
            return;

        var searchView = ReadWebFile("Views", "Hotels", "Search.cshtml");

        // Ana sayfadan gelen GET arama parametreleri korunmali.
        AssertContainsAny(searchView, "method=\"get\"", "method='get'");
        AssertContainsAny(searchView, "name=\"city\"", "name='city'", "asp-for=\"City\"", "asp-for='City'", "City");
        AssertContainsAny(searchView, "name=\"checkIn\"", "name='checkIn'", "asp-for=\"CheckIn\"", "asp-for='CheckIn'", "CheckIn");
        AssertContainsAny(searchView, "name=\"checkOut\"", "name='checkOut'", "asp-for=\"CheckOut\"", "asp-for='CheckOut'", "CheckOut");
        AssertContainsAny(searchView, "name=\"guests\"", "name='guests'", "asp-for=\"Guests\"", "asp-for='Guests'", "Guests");

        // S3-04: filtre, siralama ve sayfalama UI'i.
        AssertContainsAny(searchView, "minStarRating", "StarRating", "Yildiz", "Yıldız");
        AssertContainsAny(searchView, "rating", "puan", "AvgRating", "MinRating");
        AssertContainsAny(searchView, "sortBy", "SortBy", "sirala", "sırala");
        AssertContainsAny(searchView, "sortDir", "SortDir", "asc", "desc");
        AssertContainsAny(searchView, "_Pagination", "Pagination", "page", "Page");

        // Arama sonuclari reusable otel kartlariyla render edilmeli.
        Assert.Contains("_HotelCard", searchView);
    }

    [Fact]
    public void HotelCardAndStarRatingPartials_ShouldSupportSearchResultCards()
    {
        if (!Sprint3CardGuards.S304_SearchPageImplemented())
            return;

        var hotelCard = ReadFirstExistingWebFile(
            Path.Combine("Views", "Shared", "_HotelCard.cshtml"),
            Path.Combine("Views", "Hotels", "_HotelCard.cshtml"));
        var starRating = ReadFirstExistingWebFile(
            Path.Combine("Views", "Shared", "_StarRating.cshtml"),
            Path.Combine("Views", "Hotels", "_StarRating.cshtml"));

        AssertContainsAll(hotelCard, "Name", "City");
        AssertContainsAny(hotelCard, "Country", "Address");
        AssertContainsAny(hotelCard, "PricePerNight", "MinPrice", "Price", "Fiyat");
        AssertContainsAny(hotelCard, "_StarRating", "StarRating", "Yildiz", "Yıldız");
        AssertContainsAny(hotelCard, "asp-action=\"Detail\"", "asp-action='Detail'", "/Hotels/Detail", "Details");

        AssertContainsAny(starRating, "Model", "StarRating", "Rating");
        Assert.True(
            ContainsAny(starRating, "for", "foreach", "Enumerable.Range", "★", "fa-star", "bi-star"),
            "_StarRating partial dolu/bos yildizlari render edecek bir dongu veya yildiz ikonu icermeli.");
    }

    [Fact]
    public void SearchPage_ShouldHaveSearchSpecificStyling()
    {
        if (!Sprint3CardGuards.S304_SearchPageImplemented())
            return;

        var searchView = ReadWebFile("Views", "Hotels", "Search.cshtml");
        var siteCssPath = Path.Combine(FindWebProjectRoot(), "wwwroot", "css", "site.css");
        var siteCss = File.Exists(siteCssPath) ? File.ReadAllText(siteCssPath) : string.Empty;
        var styleEvidence = searchView + Environment.NewLine + siteCss;

        AssertContainsAny(styleEvidence, "search", "arama");
        AssertContainsAny(styleEvidence, "filter", "filtre");
        AssertContainsAny(styleEvidence, "result", "sonuc", "sonuç", "hotel-card", "otel-card");
    }

    private static Type GetRequiredType(string fullName)
    {
        return WebAssembly.GetType(fullName)
            ?? throw new InvalidOperationException($"{fullName} tipi bulunamadi.");
    }

    private static bool RequiresAuthorization(Type controllerType, MethodInfo? action)
    {
        return controllerType.GetCustomAttribute<AuthorizeAttribute>() is not null
            || action?.GetCustomAttribute<AuthorizeAttribute>() is not null;
    }

    private static bool AllowsAnonymous(MethodInfo? action)
    {
        return action?.GetCustomAttribute<AllowAnonymousAttribute>() is not null;
    }

    private static void AssertContainsAll(string text, params string[] expectedParts)
    {
        foreach (var expected in expectedParts)
        {
            Assert.Contains(expected, text, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            ContainsAny(text, expectedParts),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }

    private static bool ContainsAny(string text, params string[] expectedParts)
    {
        return expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase));
    }

    private static string ReadWebFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindWebProjectRoot() }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    private static string ReadFirstExistingWebFile(params string[] relativePaths)
    {
        var root = FindWebProjectRoot();
        var existingPath = relativePaths
            .Select(relativePath => Path.Combine(root, relativePath))
            .FirstOrDefault(File.Exists);

        Assert.False(string.IsNullOrWhiteSpace(existingPath), $"Dosya bulunamadi. Aranan yollar: {string.Join(", ", relativePaths)}");
        return File.ReadAllText(existingPath!);
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
