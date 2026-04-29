using ApiHotelsController = HotelApp.API.Controllers.HotelsController;
using HotelApp.Web.Controllers;
using System.Reflection;

namespace HotelApp.Tests;

internal static class Sprint3CardGuards
{
    private static readonly Assembly ApiAssembly = typeof(ApiHotelsController).Assembly;
    private static readonly Assembly WebAssembly = typeof(HomeController).Assembly;

    // S3-03 - Ana Sayfa karti. HomePageViewModel bu kartin en dar ve kart-spesifik belirtecidir.
    public static bool S303_HomePageImplemented()
    {
        return WebAssembly.GetType("HotelApp.Web.Models.HomePageViewModel") is not null;
    }

    // S3-04 - Otel Arama Sayfasi karti. Search.cshtml bu kartin ana ciktisidir.
    public static bool S304_SearchPageImplemented()
    {
        return File.Exists(Path.Combine(FindWebProjectRoot(), "Views", "Hotels", "Search.cshtml"));
    }

    // S3-05 - Otel Detay Sayfasi karti. Detail.cshtml bu kartin ana ve Hilal'e ait ciktisidir.
    public static bool S305_HotelDetailImplemented()
    {
        return File.Exists(Path.Combine(FindWebProjectRoot(), "Views", "Hotels", "Detail.cshtml"));
    }

    // S3-06 - Review backend + detail partial hazirligi karti. ReviewController bu karti tetikleyen dar API belirtecidir.
    public static bool S306_ReviewAndDetailPartialsImplemented()
    {
        return ApiAssembly.GetType("HotelApp.API.Controllers.ReviewController") is not null;
    }

    // S3-07 - Reservation API karti. ReservationController bu karti tetikleyen dar API belirtecidir.
    public static bool S307_ReservationApiImplemented()
    {
        return ApiAssembly.GetType("HotelApp.API.Controllers.ReservationController") is not null;
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
