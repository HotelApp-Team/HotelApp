namespace HotelApp.Tests;

// Sprint5PublicHotelPolishContractTests sinifinin amaci, S5-04 public otel akisi polish ve bug fix sozlesmesini korumaktir.
// Detail/Search UI dosyalari yoksa CI passed gecer; dosyalar olusunca planlanan mapping, filtre, partial ve responsive davranislarini dogrular.
// SOLID: SRP kullanir; sadece S5-04 otel detay, arama/filtre ve demo seed contract kontrollerinden sorumludur.
public class Sprint5PublicHotelPolishContractTests
{
    // S5-04 icin HotelsController.Detail action'inin master VM ve ilgili public verileri mapledigini kontrol eder.
    [Fact]
    public void S504_DetailController_ShouldMapMasterVmAndRelatedPublicHotelData()
    {
        // Detail sayfasi baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S504_DetailPageImplemented())
            return;

        var controller = ReadWebFile("Controllers", "HotelsController.cs");
        var model = ReadWebFile("Models", "HotelDetailPageVm.cs");
        var source = controller + Environment.NewLine + model;

        AssertContainsAny(controller, "Detail(int id", "DetailAsync", "IActionResult Detail");
        AssertContainsAny(controller, "GetAsync<HotelDetailPageVm>", "HotelDetailPageVm");
        AssertContainsAny(controller, "Hotels/{id}", "hotels/{id}", "api/hotels");
        AssertContainsAny(controller, "images", "HotelImageViewModel");
        AssertContainsAny(controller, "rooms", "AvailableRoomViewModel");
        AssertContainsAny(controller, "reviews", "ReviewViewModel");
        AssertContainsAny(controller, "BookingForm", "CreateReservationViewModel");
        AssertContainsAny(controller, "IsAuthenticated", "JWToken", "Session");
        AssertContainsAny(controller, "TempData", "Error", "RedirectToAction", "new List");

        AssertContainsAny(source, "List<AmenityViewModel>", "Amenities");
        AssertContainsAny(source, "List<HotelImageViewModel>", "Images");
        AssertContainsAny(source, "List<AvailableRoomViewModel>", "Rooms");
        AssertContainsAny(source, "List<ReviewViewModel>", "Reviews");
        AssertContainsAny(source, "CreateReservationViewModel", "BookingForm");
        AssertContainsAny(source, "bool IsAuthenticated", "IsAuthenticated");
    }

    // S5-04 icin Detail view'in galeri, olanak, oda, yorum ve rezervasyon akislarini birlikte render ettigini kontrol eder.
    [Fact]
    public void S504_DetailView_ShouldRenderGalleryAmenitiesRoomsReviewsAndBookingStates()
    {
        // Detail sayfasi baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S504_DetailPageImplemented())
            return;

        var detail = ReadWebFile("Views", "Hotels", "Detail.cshtml");
        var roomCard = ReadWebFile("Views", "Hotels", "_RoomCard.cshtml");
        var bookingModal = ReadWebFile("Views", "Hotels", "_BookingModal.cshtml");
        var reviewCard = ReadWebFile("Views", "Hotels", "_ReviewCard.cshtml");
        var source = string.Join(Environment.NewLine, detail, roomCard, bookingModal, reviewCard);

        AssertContainsAny(detail, "@model HotelApp.Web.Models.HotelDetailPageVm", "HotelDetailPageVm");
        AssertContainsAny(detail, "Model.Images", "gallery", "Galeri");
        AssertContainsAny(detail, "no-image", "placeholder", "Gorsel bulunamadi", "Görsel bulunamadı");
        AssertContainsAny(detail, "Model.Amenities", "amenity-list", "Olanak");
        AssertContainsAny(detail, "Olanak bilgisi", "Olanak bulunamadi", "henüz eklenmemiş", "henuz eklenmemis");
        AssertContainsAny(detail, "Model.Rooms", "_RoomCard", "Odalar");
        AssertContainsAny(detail, "oda bilgisi", "Uygun Odaları Gör", "Uygun Odalari Gor");
        AssertContainsAny(detail, "Model.Reviews", "_ReviewCard", "Yorumlar");
        AssertContainsAny(detail, "yorum", "Yorum bulunamadi", "henüz yorum", "henuz yorum");
        AssertContainsAny(source, "_BookingModal", "bookingModal", "Rezervasyon");
        AssertContainsAny(detail, "Login", "Giriş", "Giris", "returnUrl");
        AssertContainsAny(detail, "container", "row", "col-lg-8", "col-lg-4", "card-box");
        AssertContainsAny(detail, "carousel", "lightbox", "gallery-grid", "hotel-hero");
    }

    // S5-04 icin rezervasyon ve yorum partial'larinin dogru model ve form sozlesmesiyle calistigini kontrol eder.
    [Fact]
    public void S504_DetailPartials_ShouldUseCorrectModelsAndSafeBookingModalContract()
    {
        // Detail sayfasi baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S504_DetailPageImplemented())
            return;

        var roomCard = ReadWebFile("Views", "Hotels", "_RoomCard.cshtml");
        var bookingModal = ReadWebFile("Views", "Hotels", "_BookingModal.cshtml");
        var reviewCard = ReadWebFile("Views", "Hotels", "_ReviewCard.cshtml");

        AssertContainsAny(roomCard, "@model AvailableRoomViewModel", "AvailableRoomViewModel");
        AssertContainsAny(roomCard, "HotelRoomId", "Model.Id", "roomId");
        AssertContainsAny(roomCard, "PricePerNight", "MaxGuests", "AvailableRooms", "TotalRooms");
        AssertContainsAny(roomCard, "data-bs-toggle=\"modal\"", "data-bs-target", "bookingModal");
        AssertContainsAny(roomCard, "_BookingModal");

        // Cancel akisina karar verildiyse form asp-controller="Booking"'a yonlendirilir; Reservations da kabul edilir.
        AssertContainsAny(bookingModal, "asp-controller=\"Reservations\"", "asp-controller=\"Booking\"", "Reservations", "Booking");
        AssertContainsAny(bookingModal, "asp-action=\"Create\"", "Create");
        AssertContainsAny(bookingModal, "HotelId", "HotelRoomId");
        AssertContainsAny(bookingModal, "CheckInDate", "CheckOutDate", "GuestCount");
        AssertContainsAny(bookingModal, "max=\"@Model.MaxGuests\"", "MaxGuests");
        AssertContainsAny(bookingModal, "PaymentMethod", "CreditCard", "Cash", "Transfer");
        AssertContainsAny(bookingModal, "data-bs-dismiss", "btn-close", "required");

        AssertContainsAny(reviewCard, "@model ReviewViewModel", "ReviewViewModel");
        AssertContainsAny(reviewCard, "UserName", "Rating", "Comment", "CreatedAt");
        AssertContainsAny(reviewCard, "for (var i = 1; i <= 5", "Rating");
    }

    // S5-04 icin Search action'inin E07 parametrelerini API'ye dogru query olarak gonderdigini kontrol eder.
    [Fact]
    public void S504_SearchController_ShouldForwardFilterSortPagingAndAvailabilityParameters()
    {
        // Arama/filtre ekrani baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S504_SearchFilterUiImplemented())
            return;

        var controller = ReadWebFile("Controllers", "HotelsController.cs");

        AssertContainsAny(controller, "Search(", "SearchAsync");
        AssertContainsAny(controller, "city", "starRating", "minStarRating");
        AssertContainsAny(controller, "checkIn", "checkOut", "guestCount");
        AssertContainsAny(controller, "sortBy", "page", "pageSize");
        AssertContainsAny(controller, "hasRequiredFilters", "!string.IsNullOrWhiteSpace(city)", "guestCount.HasValue");
        AssertContainsAny(controller, "hotels/search?", "api/hotels/search", "hotels/search");
        AssertContainsAny(controller, "yyyy-MM-dd", "ToString(\"yyyy-MM-dd\")");
        AssertContainsAny(controller, "Uri.EscapeDataString", "EscapeDataString");
        AssertContainsAny(controller, "GetAsync<HotelSearchResponseViewModel>", "HotelSearchResponseViewModel");
        AssertContainsAny(controller, "TotalCount", "Hotels", "CurrentPage", "PageSize");
        AssertContainsAny(controller, "TempData[\"SearchError\"]", "SearchError", "response.Message");
    }

    // S5-04 icin Search view ve sonuc kartinin filtreleri, siralama ve sayfalama parametrelerini korudugunu kontrol eder.
    [Fact]
    public void S504_SearchView_ShouldRenderFiltersSortingPaginationAndHotelCards()
    {
        // Arama/filtre ekrani baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S504_SearchFilterUiImplemented())
            return;

        var searchView = ReadWebFile("Views", "Hotels", "Search.cshtml");
        var hotelCard = ReadWebFile("Views", "Shared", "_HotelCard.cshtml");
        var source = searchView + Environment.NewLine + hotelCard;

        AssertContainsAny(searchView, "name=\"city\"", "asp-for=\"City\"", "Model.City");
        AssertContainsAny(searchView, "name=\"starRating\"", "Model.StarRating");
        AssertContainsAny(searchView, "name=\"minStarRating\"", "Model.MinRating");
        AssertContainsAny(searchView, "name=\"checkIn\"", "Model.CheckIn", "yyyy-MM-dd");
        AssertContainsAny(searchView, "name=\"checkOut\"", "Model.CheckOut", "yyyy-MM-dd");
        AssertContainsAny(searchView, "name=\"guestCount\"", "Model.GuestCount", "min=\"1\"");
        AssertContainsAny(searchView, "name=\"sortBy\"", "price_asc", "price_desc", "rating_desc");
        AssertContainsAny(searchView, "_HotelCard", "Model.Hotels");
        AssertContainsAny(searchView, "pagination", "asp-route-page", "CurrentPage", "TotalCount");
        AssertContainsAny(searchView, "asp-route-city", "asp-route-checkIn", "asp-route-checkOut", "asp-route-guestCount", "asp-route-sortBy");
        AssertContainsAny(searchView, "alert", "uygun", "Otel aramak", "kriter");
        AssertContainsAny(searchView, "container", "row", "col-md-", "form-control", "form-select");

        AssertContainsAny(source, "MainImageUrl", "no-image");
        AssertContainsAny(source, "MinPrice", "Price", "TL");
        AssertContainsAny(source, "AvgRating", "_StarRating", "StarRating");
        AssertContainsAny(source, "City", "Country");
        AssertContainsAny(source, "asp-controller=\"Hotels\"", "asp-action=\"Detail\"", "asp-route-id");
    }

    // S5-04 public otel view'larinda hassas veri gosterilmedigini ve responsive izlerin korundugunu kontrol eder.
    [Fact]
    public void S504_PublicHotelViews_ShouldAvoidSensitiveDataAndKeepResponsiveMarkup()
    {
        // Public otel akisi baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S504_PublicHotelFlowImplemented())
            return;

        var sources = new[]
        {
            ReadOptionalWebFile("Views", "Hotels", "Detail.cshtml"),
            ReadOptionalWebFile("Views", "Hotels", "Search.cshtml"),
            ReadOptionalWebFile("Views", "Shared", "_HotelCard.cshtml"),
            ReadOptionalWebFile("Views", "Hotels", "_RoomCard.cshtml"),
            ReadOptionalWebFile("Views", "Hotels", "_BookingModal.cshtml"),
            ReadOptionalWebFile("Views", "Hotels", "_ReviewCard.cshtml")
        };
        var source = string.Join(Environment.NewLine, sources);

        AssertContainsAny(source, "container", "row", "col-", "col-md-", "col-lg-", "w-100");
        AssertContainsAny(source, "alert", "text-muted", "btn", "card", "modal");
        Assert.DoesNotContain("PasswordHash", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("EmailVerificationToken", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("FailedLoginAttempts", source, StringComparison.OrdinalIgnoreCase);
    }

    // S5-04 icin demo seed dosyasi eklenirse en az 10 otel, gorsel ve olanak verisi oldugunu kontrol eder.
    [Fact]
    public void S504_DemoSeedData_ShouldContainEnoughHotelsImagesAndAmenitiesWhenStarted()
    {
        // Demo seed dosyasi proje altinda henuz yoksa CI passed gecer; eklendiginde contract aktif olur.
        if (!Sprint5CardGuards.S504_DemoSeedDataImplemented())
            return;

        var seedSource = string.Join(Environment.NewLine, ReadDemoSeedSources());

        Assert.True(CountInsertRows(seedSource, "Hotels") >= 10, "S5-04 demo seed data en az 10 otel icermeli.");
        AssertContainsAny(seedSource, "INSERT INTO HotelImages", "INSERT [dbo].[HotelImages]", "HotelImages");
        AssertContainsAny(seedSource, "INSERT INTO Amenities", "INSERT [dbo].[Amenities]", "Amenities");
        AssertContainsAny(seedSource, "INSERT INTO HotelAmenities", "INSERT [dbo].[HotelAmenities]", "HotelAmenities");
    }

    // Web projesindeki zorunlu bir dosyayi okur.
    private static string ReadWebFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.Web") }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    // Web projesindeki opsiyonel bir dosyayi varsa okur.
    private static string ReadOptionalWebFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.Web") }.Concat(relativePathParts).ToArray());

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    // Demo seed SQL kaynaklarini test seed dosyalari haric tutarak okur.
    private static IEnumerable<string> ReadDemoSeedSources()
    {
        var solutionRoot = FindSolutionRoot();

        return Directory.GetFiles(solutionRoot, "*.sql", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                && !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                && !path.Contains($"{Path.DirectorySeparatorChar}HotelApp.Tests{Path.DirectorySeparatorChar}"))
            .Select(File.ReadAllText);
    }

    // INSERT bloklarindaki satir sayisini yaklasik olarak sayar.
    private static int CountInsertRows(string source, string tableName)
    {
        var count = 0;
        var inTargetInsert = false;

        foreach (var rawLine in source.Split(Environment.NewLine))
        {
            var line = rawLine.Trim();

            if (line.StartsWith("GO", StringComparison.OrdinalIgnoreCase) || line.EndsWith(";", StringComparison.Ordinal))
            {
                if (inTargetInsert && line.StartsWith("(", StringComparison.Ordinal))
                    count++;

                if (line.StartsWith("GO", StringComparison.OrdinalIgnoreCase))
                    inTargetInsert = false;

                continue;
            }

            if (line.Contains($"INSERT INTO {tableName}", StringComparison.OrdinalIgnoreCase)
                || line.Contains($"INSERT [dbo].[{tableName}]", StringComparison.OrdinalIgnoreCase))
            {
                inTargetInsert = true;
                continue;
            }

            if (inTargetInsert && line.StartsWith("(", StringComparison.Ordinal))
                count++;
        }

        return count;
    }

    // Test calisma klasorunden yukari cikarak istenen proje klasorunu bulur.
    private static string FindProjectRoot(string projectName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, projectName);
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"{projectName} klasoru bulunamadi.");
    }

    // Test calisma klasorunden yukari cikarak solution kok klasorunu bulur.
    private static string FindSolutionRoot()
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

    // Metin icinde beklenen parcalardan en az birinin gectigini kontrol eder.
    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase)),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }
}
