using HotelApp.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace HotelApp.Tests;

public class Sprint3HotelDetailPageContractTests
{
    private static readonly Assembly WebAssembly = typeof(HomeController).Assembly;

    [Fact]
    public void HotelsController_ShouldExposePublicDetailActionAndRequestHotelDetailData()
    {
        if (!Sprint3CardGuards.S305_HotelDetailImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.Web.Controllers.HotelsController");
        var detailActions = controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(method => method.Name == "Detail")
            .ToList();

        Assert.NotEmpty(detailActions);
        Assert.Contains(detailActions, action => HasRouteIdParameter(action));
        Assert.True(
            detailActions.Any(AllowsAnonymous) || !RequiresAuthorization(controllerType, detailActions),
            "HotelsController.Detail public otel detay sayfasi olmali; auth zorunlulugu olmamali veya action [AllowAnonymous] olmali.");

        var source = ReadWebFile("Controllers", "HotelsController.cs");

        AssertContainsAny(source, "hotels/", "api/hotels/", "GetHotel", "GetById");
        AssertContainsAny(source, "ApiService", "GetAsync", "HttpClient");
        AssertContainsAny(source, "Detail", "View(");
    }

    [Fact]
    public void HotelDetailPageVm_ShouldExposeS305AndGroupCIntegrationContract()
    {
        if (!Sprint3CardGuards.S305_HotelDetailImplemented())
            return;

        var vmType = GetRequiredType("HotelApp.Web.Models.HotelDetailPageVm");

        AssertHasProperty(vmType, "Id", typeof(int));
        AssertHasProperty(vmType, "Name", typeof(string));
        AssertHasAnyProperty(vmType, "City", "Country", "Address", "Location");
        AssertHasAnyProperty(vmType, "Description", "ShortDescription", "Overview");
        AssertHasAnyProperty(vmType, "StarRating", "Stars");
        AssertHasAnyProperty(vmType, "AvgRating", "AverageRating", "Rating");
        AssertHasAnyProperty(vmType, "Images", "GalleryImages", "ImageUrls", "MainImageUrl", "HotelImages");
        AssertHasAnyProperty(vmType, "Amenities", "HotelAmenities", "Facilities");

        // S3-05 karti Grup C entegrasyonu icin HotelDetailPageVm veri alanlarinin netlesmesini istiyor.
        AssertHasProperty(vmType, "Rooms");
        AssertHasProperty(vmType, "Reviews");
        AssertHasProperty(vmType, "BookingForm");
        AssertHasProperty(vmType, "IsAuthenticated", typeof(bool));
    }

    [Fact]
    public void DetailView_ShouldRenderBasicInfoGalleryAndGroupCSkeletonFromCard()
    {
        if (!Sprint3CardGuards.S305_HotelDetailImplemented())
            return;

        var detailView = ReadWebFile("Views", "Hotels", "Detail.cshtml");

        AssertContainsAny(detailView, "HotelDetailPageVm", "@model");
        AssertContainsAll(detailView, "Name");
        AssertContainsAny(detailView, "City", "Country", "Address", "Location");
        AssertContainsAny(detailView, "Description", "Açıklama", "Aciklama", "Overview");
        AssertContainsAny(detailView, "_StarRating", "StarRating", "AvgRating", "AverageRating", "Rating");

        // S3-05: gorsel galeri bolumunun ilk versiyonu.
        AssertContainsAny(detailView, "Gallery", "Galeri", "carousel", "Images", "GalleryImages", "MainImageUrl", "<img");

        // Grup C partial entegrasyonuna uygun iskelet hazir olmali.
        AssertContainsAny(detailView, "Rooms", "_RoomCard", "Oda", "Room");
        AssertContainsAny(detailView, "Reviews", "_ReviewCard", "Yorum", "Review");
        AssertContainsAny(detailView, "Booking", "_BookingModal", "Rezervasyon", "Reserve");
    }

    [Fact]
    public void DetailPage_ShouldHavePageSpecificStyling()
    {
        if (!Sprint3CardGuards.S305_HotelDetailImplemented())
            return;

        var detailView = ReadWebFile("Views", "Hotels", "Detail.cshtml");
        var siteCssPath = Path.Combine(FindWebProjectRoot(), "wwwroot", "css", "site.css");
        var siteCss = File.Exists(siteCssPath) ? File.ReadAllText(siteCssPath) : string.Empty;
        var styleEvidence = detailView + Environment.NewLine + siteCss;

        AssertContainsAny(styleEvidence, "detail", "hotel-detail", "otel-detay");
        AssertContainsAny(styleEvidence, "gallery", "galeri", "carousel");
        AssertContainsAny(styleEvidence, "room", "review", "booking", "oda", "yorum", "rezervasyon");
    }

    [Fact]
    public void HotelCard_ShouldNavigateToDetailForHomeSearchDetailFlow()
    {
        if (!Sprint3CardGuards.S303_HomePageImplemented())
            return;

        var hotelCard = ReadFirstExistingWebFile(
            Path.Combine("Views", "Shared", "_HotelCard.cshtml"),
            Path.Combine("Views", "Hotels", "_HotelCard.cshtml"));

        AssertContainsAny(hotelCard, "asp-controller=\"Hotels\"", "asp-controller='Hotels'", "/Hotels/");
        AssertContainsAny(hotelCard, "asp-action=\"Detail\"", "asp-action='Detail'", "/Hotels/Detail", "Detail");
        AssertContainsAny(hotelCard, "asp-route-id", "Model.Id", "HotelId");
    }

    private static Type GetRequiredType(string fullName)
    {
        return WebAssembly.GetType(fullName)
            ?? throw new InvalidOperationException($"{fullName} tipi bulunamadi.");
    }

    private static bool HasRouteIdParameter(MethodInfo action)
    {
        return action.GetParameters().Any(parameter =>
            string.Equals(parameter.Name, "id", StringComparison.OrdinalIgnoreCase)
            && (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(int?)));
    }

    private static bool RequiresAuthorization(Type controllerType, IEnumerable<MethodInfo> actions)
    {
        return controllerType.GetCustomAttribute<AuthorizeAttribute>() is not null
            || actions.Any(action => action.GetCustomAttribute<AuthorizeAttribute>() is not null);
    }

    private static bool AllowsAnonymous(MethodInfo action)
    {
        return action.GetCustomAttribute<AllowAnonymousAttribute>() is not null;
    }

    private static void AssertHasProperty(Type type, string propertyName, Type? expectedType = null)
    {
        var property = type.GetProperty(propertyName);

        Assert.NotNull(property);

        if (expectedType is not null)
            Assert.Equal(expectedType, property.PropertyType);
    }

    private static void AssertHasAnyProperty(Type type, params string[] propertyNames)
    {
        Assert.True(
            propertyNames.Any(name => type.GetProperty(name) is not null),
            $"{type.Name} icinde beklenen alanlardan en az biri bulunmali: {string.Join(", ", propertyNames)}");
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
            expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase)),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
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
