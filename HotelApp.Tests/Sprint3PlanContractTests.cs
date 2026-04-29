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
    private static readonly Assembly ApiAssembly = typeof(ApiHotelsController).Assembly;
    private static readonly Assembly DomainAssembly = typeof(Hotel).Assembly;
    private static readonly Assembly WebAssembly = typeof(WebHomeController).Assembly;

    [Fact]
    public void ReservationsController_ShouldExposeE23ToE27RoutesFromPlan()
    {
        if (!Sprint3ImplementationExists())
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
        if (!Sprint3ImplementationExists())
            return;

        var controllerType = GetRequiredType(ApiAssembly, "HotelApp.API.Controllers.ReviewController");

        Assert.Equal("api/reviews", GetControllerRoute(controllerType));

        Assert.Contains(GetHttpAttributes<HttpPostAttribute>(controllerType), action => IsEmptyRoute(action.Attribute.Template));
        Assert.Contains(GetHttpAttributes<HttpGetAttribute>(controllerType), action => action.Attribute.Template == "~/api/hotels/{hotelId}/reviews");
        Assert.Contains(GetHttpAttributes<HttpDeleteAttribute>(controllerType), action => action.Attribute.Template == "{id}");
        Assert.Contains(GetHttpAttributes<HttpGetAttribute>(controllerType), action => action.Attribute.Template == "my");
    }

    [Fact]
    public void HotelReviewsListEndpoint_ShouldAllowAnonymousAccess()
    {
        if (!Sprint3ImplementationExists())
            return;

        var controllerType = GetRequiredType(ApiAssembly, "HotelApp.API.Controllers.ReviewController");
        var hotelReviewsAction = GetHttpAttributes<HttpGetAttribute>(controllerType)
            .SingleOrDefault(action => action.Attribute.Template == "~/api/hotels/{hotelId}/reviews");

        Assert.NotNull(hotelReviewsAction.Method);
        Assert.NotNull(hotelReviewsAction.Method.GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Fact]
    public void ReservationDto_ShouldContainPlanResponseFields()
    {
        if (!Sprint3ImplementationExists())
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
        if (!Sprint3ImplementationExists())
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
        if (!Sprint3ImplementationExists())
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
    public void HotelDetailIntegration_ShouldContainPartialContractFromSprint3Plan()
    {
        if (!Sprint3ImplementationExists())
            return;

        var root = FindRepositoryRoot();
        var views = Path.Combine(root, "HotelApp.Web", "Views", "Hotels");
        var detailsViewPath = Path.Combine(views, "Details.cshtml");
        var roomCardPath = Path.Combine(views, "_RoomCard.cshtml");

        Assert.True(File.Exists(detailsViewPath), "Views/Hotels/Details.cshtml bulunmali.");
        Assert.True(File.Exists(roomCardPath), "_RoomCard partial bulunmali.");
        Assert.True(File.Exists(Path.Combine(views, "_BookingModal.cshtml")), "_BookingModal partial bulunmali.");
        Assert.True(File.Exists(Path.Combine(views, "_ReviewCard.cshtml")), "_ReviewCard partial bulunmali.");

        var detailsView = File.ReadAllText(detailsViewPath);
        var roomCardView = File.ReadAllText(roomCardPath);

        Assert.Contains("_RoomCard", detailsView);
        Assert.Contains("_ReviewCard", detailsView);
        Assert.Contains("_BookingModal", roomCardView);
    }

    [Fact]
    public void HotelDetailPageVm_ShouldExposeGroupBIntegrationContract()
    {
        var vmType = WebAssembly.GetType("HotelApp.Web.Models.HotelDetailPageVm");
        if (vmType is null)
            return;

        AssertHasProperty(vmType, "Rooms");
        AssertHasProperty(vmType, "Reviews");
        AssertHasProperty(vmType, "BookingForm");
        AssertHasProperty(vmType, "IsAuthenticated", typeof(bool));
    }

    private static bool Sprint3ImplementationExists()
    {
        return ApiAssembly.GetType("HotelApp.API.Controllers.ReservationController") is not null
            || ApiAssembly.GetType("HotelApp.API.Controllers.ReviewController") is not null
            || DomainAssembly.GetType("HotelApp.Domain.DTOs.ReservationDto") is not null
            || DomainAssembly.GetType("HotelApp.Domain.DTOs.ReviewDto") is not null;
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

    private static string ReadSourceFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindRepositoryRoot() }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Kaynak dosya bulunamadi: {path}");
        return File.ReadAllText(path);
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
