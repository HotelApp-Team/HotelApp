using WebHomeController = HotelApp.Web.Controllers.HomeController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HotelApp.Tests;

public class Sprint4HotelDetailIntegrationContractTests
{
    private static readonly Assembly WebAssembly = typeof(WebHomeController).Assembly;

    [Fact]
    public void S405_HotelsControllerDetail_ShouldBuildMasterViewModelFromPlanDataSources()
    {
        if (!Sprint4CardGuards.S405_HotelDetailMasterMappingImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.Web.Controllers.HotelsController");
        var detailAction = FindAction(controllerType, "Detail");

        Assert.NotNull(detailAction);
        Assert.True(HasIdParameter(detailAction!), "HotelsController.Detail id parametresi almali.");
        Assert.False(HasAuthorize(controllerType, detailAction!), "Otel detay sayfasi public olmali; auth zorunlu olmamali.");

        var source = ReadWebFile("Controllers", "HotelsController.cs");
        var detailBody = ExtractMethodBodyOrExpression(source, "Detail");

        AssertContainsAny(detailBody, "HotelDetailPageVm", "HotelDetailViewModel");
        AssertContainsAny(detailBody, "GetAsync");
        AssertContainsAny(detailBody, "Hotels/{id}", "hotels/{id}");
        AssertContainsAny(detailBody, "images", "Images");
        AssertContainsAny(detailBody, "rooms", "Rooms");
        AssertContainsAny(detailBody, "reviews", "Reviews");
        AssertContainsAny(detailBody, "BookingForm");
        AssertContainsAny(detailBody, "IsAuthenticated", "JWToken");
        AssertContainsAny(detailBody, "return View", "View(\"Detail\"", "View(model)");
    }

    [Fact]
    public void S405_HotelDetailPageVm_ShouldExposeMasterContractFromPlan()
    {
        if (!Sprint4CardGuards.S405_HotelDetailMasterMappingImplemented())
            return;

        var vmType = GetRequiredType("HotelApp.Web.Models.HotelDetailPageVm");

        AssertHasAnyProperty(vmType, "Hotel", "Id", "HotelId");
        AssertHasAnyProperty(vmType, "Hotel", "Name", "HotelName");
        AssertHasAnyProperty(vmType, "Images", "GalleryImages", "HotelImages");
        AssertHasAnyProperty(vmType, "Amenities", "HotelAmenities", "Facilities");
        AssertHasEnumerableProperty(vmType, "Rooms", "HotelRooms", "AvailableRooms");
        AssertHasEnumerableProperty(vmType, "Reviews", "HotelReviews");
        AssertHasAnyProperty(vmType, "BookingForm", "ReservationForm", "CreateReservation");

        var authProperty = GetAnyProperty(vmType, "IsAuthenticated", "Authenticated");
        Assert.NotNull(authProperty);
        Assert.Equal(typeof(bool), authProperty!.PropertyType);
    }

    [Fact]
    public void S405_DetailView_ShouldKeepGalleryAndMasterModelSkeleton()
    {
        if (!Sprint4CardGuards.S405_HotelDetailMasterMappingImplemented())
            return;

        var detailView = ReadWebFile("Views", "Hotels", "Detail.cshtml");

        AssertContainsAny(detailView, "HotelDetailPageVm", "@model");
        AssertContainsAny(detailView, "Model.Name", "Model.HotelName", "Model.Hotel");
        AssertContainsAny(detailView, "Model.Images", "Gallery", "Galeri", "<img");
        AssertContainsAny(detailView, "Model.Amenities", "Olanak", "Amenit", "Facilities");
        AssertContainsAny(detailView, "Rezervasyon", "Booking", "BookingForm");
    }

    [Fact]
    public void S405_Partials_ShouldMatchRoomReviewBookingOwnerContracts()
    {
        if (!Sprint4CardGuards.S405_DetailPartialsPrepared())
            return;

        var roomCard = ReadFirstExistingWebFile(
            Path.Combine("Views", "Hotels", "_RoomCard.cshtml"),
            Path.Combine("Views", "Shared", "_RoomCard.cshtml"));

        var reviewCard = ReadFirstExistingWebFile(
            Path.Combine("Views", "Hotels", "_ReviewCard.cshtml"),
            Path.Combine("Views", "Shared", "_ReviewCard.cshtml"));

        var bookingModal = ReadFirstExistingWebFile(
            Path.Combine("Views", "Hotels", "_BookingModal.cshtml"),
            Path.Combine("Views", "Shared", "_BookingModal.cshtml"));

        AssertContainsAny(roomCard, "AvailableRoomViewModel", "RoomVM", "RoomViewModel");
        AssertContainsAny(roomCard, "RoomTypeName", "Name");
        AssertContainsAny(roomCard, "MaxGuests", "Kapasite", "kişi", "kisi");
        AssertContainsAny(roomCard, "PricePerNight", "Fiyat", "gecelik");
        AssertContainsAny(roomCard, "AvailableRooms", "TotalRooms", "Müsait", "Musait");
        AssertContainsAny(roomCard, "_BookingModal", "data-bs-toggle=\"modal\"", "Rezervasyon");

        AssertContainsAny(reviewCard, "ReviewViewModel", "ReviewVM");
        AssertContainsAny(reviewCard, "UserName", "Kullanıcı", "Kullanici");
        AssertContainsAny(reviewCard, "Rating", "Star", "★", "_StarRating");
        AssertContainsAny(reviewCard, "Comment", "Yorum");
        AssertContainsAny(reviewCard, "CreatedAt", "Tarih");

        AssertContainsAny(bookingModal, "AvailableRoomViewModel", "BookingVM", "Reservation");
        AssertContainsAny(bookingModal, "asp-controller=\"Reservations\"", "Reservations");
        AssertContainsAny(bookingModal, "asp-action=\"Create\"", "Create");
        AssertContainsAny(bookingModal, "method=\"post\"", "method=\"POST\"");
        AssertContainsAny(bookingModal, "HotelRoomId", "RoomId");
        AssertContainsAny(bookingModal, "CheckInDate", "CheckIn", "Giriş", "Giris");
        AssertContainsAny(bookingModal, "CheckOutDate", "CheckOut", "Çıkış", "Cikis");
        AssertContainsAny(bookingModal, "GuestCount", "Misafir");
        AssertContainsAny(bookingModal, "PaymentMethod", "Ödeme", "Odeme");
        AssertContainsAny(bookingModal, "PricePerNight", "fiyat", "Fiyat");
    }

    [Fact]
    public void S405_DetailView_ShouldRenderRoomReviewAndBookingPartialsWhenIntegrationStarts()
    {
        if (!Sprint4CardGuards.S405_DetailPartialIntegrationImplemented())
            return;

        var detailView = ReadWebFile("Views", "Hotels", "Detail.cshtml");

        AssertContainsAny(detailView, "Model.Rooms", "Rooms");
        AssertContainsAny(detailView, "_RoomCard", "RoomCard");
        AssertContainsAny(detailView, "Model.Reviews", "Reviews");
        AssertContainsAny(detailView, "_ReviewCard", "ReviewCard");
        AssertContainsAny(detailView, "_BookingModal", "_RoomCard", "BookingForm");
        AssertContainsAny(detailView, "foreach", "@for", "for (");
        AssertContainsAny(detailView, "IsAuthenticated", "Login", "Giriş", "Giris");

        Assert.DoesNotContain("Grup C burada", detailView, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("placeholder", detailView, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void S405_IntegratedBookingFlow_ShouldKeepReservationPostAndLoginAwareness()
    {
        if (!Sprint4CardGuards.S405_DetailPartialIntegrationImplemented())
            return;

        var detailView = ReadWebFile("Views", "Hotels", "Detail.cshtml");
        var roomCard = ReadFirstExistingWebFile(
            Path.Combine("Views", "Hotels", "_RoomCard.cshtml"),
            Path.Combine("Views", "Shared", "_RoomCard.cshtml"));
        var bookingModal = ReadFirstExistingWebFile(
            Path.Combine("Views", "Hotels", "_BookingModal.cshtml"),
            Path.Combine("Views", "Shared", "_BookingModal.cshtml"));

        var combined = string.Join(Environment.NewLine, detailView, roomCard, bookingModal);

        AssertContainsAny(combined, "asp-controller=\"Reservations\"", "Reservations");
        AssertContainsAny(combined, "asp-action=\"Create\"", "Create");
        AssertContainsAny(combined, "HotelRoomId", "RoomId");
        AssertContainsAny(combined, "IsAuthenticated", "JWToken", "Login", "Giriş", "Giris");
        AssertContainsAny(combined, "data-bs-toggle=\"modal\"", "modal", "BookingModal");
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

    private static bool HasIdParameter(MethodInfo action)
    {
        return action.GetParameters().Any(parameter =>
            string.Equals(parameter.Name, "id", StringComparison.OrdinalIgnoreCase)
            && (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(int?)));
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

    private static void AssertHasEnumerableProperty(Type type, params string[] propertyNames)
    {
        var property = GetAnyProperty(type, propertyNames);

        Assert.NotNull(property);
        Assert.True(
            IsEnumerableButNotString(property!.PropertyType),
            $"{type.Name}.{property.Name} liste/collection tipinde olmali.");
    }

    private static bool IsEnumerableButNotString(Type type)
    {
        return type != typeof(string)
            && type.GetInterfaces().Append(type).Any(interfaceType =>
                interfaceType.IsGenericType
                && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
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
