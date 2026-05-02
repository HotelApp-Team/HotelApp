using HotelApp.Web.Controllers;
using System.Reflection;

namespace HotelApp.Tests;

public class Sprint3HomePageContractTests
{
    private static readonly Assembly WebAssembly = typeof(HomeController).Assembly;

    [Fact]
    public void HomePageViewModel_ShouldExposeS303HomePageContract()
    {
        if (!Sprint3CardGuards.S303_HomePageImplemented())
            return;

        var homePageVm = GetRequiredType("HotelApp.Web.Models.HomePageViewModel");
        var hotelCardVm = GetRequiredType("HotelApp.Web.Models.HotelCardViewModel");

        AssertHasProperty(homePageVm, "Cities");
        AssertHasProperty(homePageVm, "FeaturedHotels");

        AssertHasProperty(hotelCardVm, "Id", typeof(int));
        AssertHasProperty(hotelCardVm, "Name", typeof(string));
        AssertHasProperty(hotelCardVm, "City", typeof(string));
        AssertHasProperty(hotelCardVm, "Country", typeof(string));
        AssertHasProperty(hotelCardVm, "StarRating");
        AssertHasProperty(hotelCardVm, "AvgRating");
        AssertHasProperty(hotelCardVm, "MainImageUrl");

        // Frontend planindaki _HotelCard sozlesmesi fiyat bilgisini de bekliyor.
        Assert.True(
            HasAnyProperty(hotelCardVm, "PricePerNight", "MinPrice", "Price"),
            "HotelCardViewModel plan geregi fiyat bilgisini tasimali: PricePerNight, MinPrice veya Price.");
    }

    [Fact]
    public void IndexView_ShouldRenderSearchFormAndFeaturedHotelsFromPlan()
    {
        if (!Sprint3CardGuards.S303_HomePageImplemented())
            return;

        var indexView = ReadWebFile("Views", "Home", "Index.cshtml");

        Assert.Contains("asp-controller=\"Hotels\"", indexView);
        Assert.Contains("asp-action=\"Search\"", indexView);
        Assert.Contains("method=\"get\"", indexView, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("name=\"city\"", indexView);
        Assert.Contains("name=\"checkIn\"", indexView);
        Assert.Contains("name=\"checkOut\"", indexView);
        Assert.Contains("name=\"guestCount\"", indexView);
        Assert.Contains("FeaturedHotels", indexView);
        Assert.Contains("_HotelCard", indexView);
    }

    [Fact]
    public void HotelCardPartial_ShouldRenderReusableCardFieldsFromPlan()
    {
        if (!Sprint3CardGuards.S303_HomePageImplemented())
            return;

        var hotelCard = ReadWebFile("Views", "Shared", "_HotelCard.cshtml");

        Assert.Contains("MainImageUrl", hotelCard);
        Assert.Contains("Name", hotelCard);
        Assert.Contains("City", hotelCard);
        Assert.Contains("Country", hotelCard);
        Assert.Contains("AvgRating", hotelCard);
        Assert.Contains("_StarRating", hotelCard);
        Assert.True(
            hotelCard.Contains("PricePerNight") || hotelCard.Contains("MinPrice") || hotelCard.Contains("Price"),
            "_HotelCard plan geregi fiyat bilgisini gostermeli.");
        Assert.Contains("asp-controller=\"Hotels\"", hotelCard);
        Assert.Contains("asp-action=\"Detail\"", hotelCard);
        Assert.Contains("asp-route-id", hotelCard);
    }

    [Fact]
    public void PublicLayout_ShouldPreserveLoginPartialWhenHomePageWorkTouchesLayout()
    {
        if (!Sprint3CardGuards.S303_HomePageImplemented())
            return;

        var layout = ReadWebFile("Views", "Shared", "_Layout.cshtml");

        Assert.Contains("_LoginPartial", layout);
    }

    private static Type GetRequiredType(string fullName)
    {
        return WebAssembly.GetType(fullName)
            ?? throw new InvalidOperationException($"{fullName} tipi bulunamadi.");
    }

    private static void AssertHasProperty(Type type, string propertyName, Type? expectedType = null)
    {
        var property = type.GetProperty(propertyName);

        Assert.NotNull(property);

        if (expectedType is not null)
            Assert.Equal(expectedType, property.PropertyType);
    }

    private static bool HasAnyProperty(Type type, params string[] propertyNames)
    {
        return propertyNames.Any(name => type.GetProperty(name) is not null);
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
