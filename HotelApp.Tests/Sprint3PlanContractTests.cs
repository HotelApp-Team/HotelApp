using HotelApp.Domain.Entities;
using ApiHotelsController = HotelApp.API.Controllers.HotelsController;
using WebHomeController = HotelApp.Web.Controllers.HomeController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace HotelApp.Tests;

public class Sprint3PlanContractTests
{
    private const string HotelReviewsRoute = "~/api/hotels/{hotelId}/reviews";

    private static readonly Assembly ApiAssembly = typeof(ApiHotelsController).Assembly;
    private static readonly Assembly DomainAssembly = typeof(Hotel).Assembly;
    private static readonly Assembly WebAssembly = typeof(WebHomeController).Assembly;

    [Fact]
    public void ReservationsController_ShouldExposeE23ToE27RoutesFromPlan()
    {
        if (!Sprint3CardGuards.S307_ReservationApiImplemented())
            return;

        var controllerType = GetRequiredType(ApiAssembly, "HotelApp.API.Controllers.ReservationController");

        Assert.Equal("api/reservations", GetControllerRoute(controllerType));
        Assert.NotNull(controllerType.GetCustomAttribute<AuthorizeAttribute>());

        Assert.Contains(GetHttpAttributes<HttpPostAttribute>(controllerType), action => IsEmptyRoute(action.Attribute.Template));
        Assert.Contains(GetHttpAttributes<HttpGetAttribute>(controllerType), action => action.Attribute.Template == "my");
        Assert.Contains(GetHttpAttributes<HttpGetAttribute>(controllerType), action => action.Attribute.Template == "{id}");
        Assert.Contains(GetHttpAttributes<HttpPutAttribute>(controllerType), action => action.Attribute.Template == "{id}/cancel");
        Assert.Contains(GetHttpAttributes<HttpGetAttribute>(controllerType), action => action.Attribute.Template == "{id}/invoice");
    }

    [Fact]
    public void ReviewsController_ShouldExposeE28ToE31RoutesFromPlan()
    {
        if (!Sprint3CardGuards.S306_ReviewAndDetailPartialsImplemented())
            return;

        var controllerType = GetRequiredType(ApiAssembly, "HotelApp.API.Controllers.ReviewController");

        Assert.Equal("api/reviews", GetControllerRoute(controllerType));

        Assert.Contains(GetHttpAttributes<HttpPostAttribute>(controllerType), action => IsEmptyRoute(action.Attribute.Template));
        Assert.Contains(GetHttpAttributes<HttpGetAttribute>(controllerType), action => action.Attribute.Template == HotelReviewsRoute);
        Assert.Contains(GetHttpAttributes<HttpDeleteAttribute>(controllerType), action => action.Attribute.Template == "{id}");
        Assert.Contains(GetHttpAttributes<HttpGetAttribute>(controllerType), action => action.Attribute.Template == "my");
    }

    [Fact]
    public void HotelReviewsListEndpoint_ShouldAllowAnonymousAccess()
    {
        if (!Sprint3CardGuards.S306_ReviewAndDetailPartialsImplemented())
            return;

        var controllerType = GetRequiredType(ApiAssembly, "HotelApp.API.Controllers.ReviewController");
        var hotelReviewsAction = GetHttpAttributes<HttpGetAttribute>(controllerType)
            .SingleOrDefault(action => action.Attribute.Template == HotelReviewsRoute);

        Assert.NotNull(hotelReviewsAction.Method);
        Assert.NotNull(hotelReviewsAction.Method.GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Fact]
    public void ReservationDto_ShouldContainPlanResponseFields()
    {
        if (!Sprint3CardGuards.S307_ReservationApiImplemented())
            return;

        var dtoType = GetRequiredType(DomainAssembly, "HotelApp.Domain.DTOs.ReservationDto");

        AssertHasProperty(dtoType, "Id", typeof(int));
        AssertHasProperty(dtoType, "UserId", typeof(int));
        AssertHasProperty(dtoType, "UserName", typeof(string));
        AssertHasProperty(dtoType, "HotelRoomId", typeof(int));
        AssertHasProperty(dtoType, "HotelName", typeof(string));
        AssertHasProperty(dtoType, "RoomTypeName", typeof(string));
        AssertHasProperty(dtoType, "CheckInDate", typeof(DateOnly));
        AssertHasProperty(dtoType, "CheckOutDate", typeof(DateOnly));
        AssertHasProperty(dtoType, "GuestCount", typeof(int));
        AssertHasProperty(dtoType, "TotalPrice", typeof(decimal));
        AssertHasProperty(dtoType, "PaymentMethod", typeof(string));
        AssertHasProperty(dtoType, "Status", typeof(string));
        AssertHasProperty(dtoType, "CreatedAt", typeof(DateTime));
    }

    [Fact]
    public void ReviewDto_ShouldContainPlanResponseFields()
    {
        if (!Sprint3CardGuards.S306_ReviewAndDetailPartialsImplemented())
            return;

        var dtoType = GetRequiredType(DomainAssembly, "HotelApp.Domain.DTOs.ReviewDto");

        AssertHasProperty(dtoType, "Id", typeof(int));
        AssertHasProperty(dtoType, "UserId", typeof(int));
        AssertHasProperty(dtoType, "UserName", typeof(string));
        AssertHasProperty(dtoType, "HotelId", typeof(int));
        AssertHasProperty(dtoType, "HotelName", typeof(string));
        AssertHasNumericProperty(dtoType, "Rating");
        AssertHasProperty(dtoType, "Comment", typeof(string));
        AssertHasProperty(dtoType, "CreatedAt", typeof(DateTime));
    }

    [Fact]
    public void ReservationAuthorizationAndCancelRules_ShouldBeRepresentedInSource()
    {
        if (!Sprint3CardGuards.S307_ReservationApiImplemented())
            return;

        var controllerSource = ReadSourceFile("HotelApp.API", "Controllers", "ReservationController.cs");
        var serviceSource = ReadSourceFile("HotelApp.Domain", "Services", "ReservationService.cs");

        Assert.Contains("IsInRole", controllerSource);
        Assert.Contains("Admin", controllerSource);
        Assert.Contains("PaymentMethod", serviceSource);
        Assert.Contains("IsNullOrWhiteSpace", serviceSource);
        Assert.Contains("DateTime.Today", serviceSource);
        Assert.Contains("Completed", serviceSource);
    }

    [Fact]
    public void GroupCDetailPartials_ShouldExistForHotelDetailIntegration()
    {
        if (!Sprint3CardGuards.S306_ReviewAndDetailPartialsImplemented())
            return;

        var roomCardView = ReadFirstExistingViewFile("_RoomCard.cshtml");
        var bookingModalView = ReadFirstExistingViewFile("_BookingModal.cshtml");
        var reviewCardView = ReadFirstExistingViewFile("_ReviewCard.cshtml");

        AssertContainsAny(roomCardView, "Room", "RoomType", "Oda", "Capacity", "Price");
        AssertContainsAny(bookingModalView, "Booking", "Reservation", "Rezervasyon", "CheckIn", "CheckOut");
        AssertContainsAny(reviewCardView, "Review", "Yorum", "Rating", "Comment");
    }

    [Fact]
    public void HotelDetailPageVm_ShouldExposeGroupBIntegrationContract()
    {
        if (!Sprint3CardGuards.S305_HotelDetailImplemented())
            return;

        var vmType = WebAssembly.GetType("HotelApp.Web.Models.HotelDetailPageVm");

        Assert.NotNull(vmType);
        AssertHasProperty(vmType, "Rooms");
        AssertHasProperty(vmType, "Reviews");
        AssertHasProperty(vmType, "BookingForm");
        AssertHasProperty(vmType, "IsAuthenticated", typeof(bool));
    }

    private static Type GetRequiredType(Assembly assembly, string fullName)
    {
        return assembly.GetType(fullName)
            ?? throw new InvalidOperationException($"{fullName} tipi bulunamadi.");
    }

    private static string? GetControllerRoute(Type controllerType)
    {
        return controllerType.GetCustomAttribute<RouteAttribute>()?.Template;
    }

    private static bool IsEmptyRoute(string? template)
    {
        return string.IsNullOrWhiteSpace(template);
    }

    private static List<(MethodInfo Method, TAttribute Attribute)> GetHttpAttributes<TAttribute>(Type controllerType)
        where TAttribute : HttpMethodAttribute
    {
        return controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .SelectMany(method => method
                .GetCustomAttributes<TAttribute>(inherit: false)
                .Select(attribute => (method, attribute)))
            .ToList();
    }

    private static void AssertHasProperty(Type type, string propertyName, Type? expectedType = null)
    {
        var property = type.GetProperty(propertyName);

        Assert.NotNull(property);

        if (expectedType is not null)
            Assert.Equal(expectedType, property.PropertyType);
    }

    private static void AssertHasNumericProperty(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName);

        Assert.NotNull(property);
        Assert.Contains(property.PropertyType, new[] { typeof(byte), typeof(short), typeof(int) });
    }

    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase)),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }

    private static string ReadSourceFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindRepositoryRoot() }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Kaynak dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    private static string ReadFirstExistingViewFile(string fileName)
    {
        var root = FindRepositoryRoot();
        var candidatePaths = new[]
        {
            Path.Combine(root, "HotelApp.Web", "Views", "Hotels", fileName),
            Path.Combine(root, "HotelApp.Web", "Views", "Shared", fileName)
        };
        var existingPath = candidatePaths.FirstOrDefault(File.Exists);

        Assert.False(string.IsNullOrWhiteSpace(existingPath), $"View dosyasi bulunamadi: {fileName}");
        return File.ReadAllText(existingPath!);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "HotelApp.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("HotelApp.sln bulunamadi.");
    }
}
