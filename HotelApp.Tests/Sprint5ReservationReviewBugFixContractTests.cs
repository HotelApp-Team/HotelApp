namespace HotelApp.Tests;

// Sprint5ReservationReviewBugFixContractTests sinifinin amaci, S5-06 Rezervasyon/Yorum Bug Fix kartinin CI sozlesmesini korumaktir.
// Ilgili rezervasyon/yorum akisi baslamadiysa CI passed gecer; kod baslayinca plan endpointleri, auth ve bos/hata durumlari dogrulanir.
// SOLID: SRP kullanir; sadece S5-06 reservation/review bug fix contract kontrollerinden sorumludur.
public class Sprint5ReservationReviewBugFixContractTests
{
    // S5-06 icin rezervasyon API'sinin E23-E27 endpoint sozlesmesini tasidigini kontrol eder.
    [Fact]
    public void S506_ReservationApi_ShouldExposeE23E24E25E26E27ContractsWhenStarted()
    {
        // Rezervasyon API baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReservationApiImplemented())
            return;

        var controller = ReadFirstExistingApiFile("Controllers", "ReservationController.cs", "Controllers", "ReservationsController.cs");
        var service = ReadOptionalDomainFile("Services", "ReservationService.cs");
        var dto = ReadOptionalDomainFile("DTOs", "ReservationDto.cs");
        var source = string.Join(Environment.NewLine, controller, service, dto);

        AssertContainsAny(controller, "[Route(\"api/reservations\")]", "api/reservations");
        AssertContainsAny(controller, "[ApiController]", "ControllerBase");
        AssertContainsAny(controller, "[Authorize]", "AuthorizeAttribute");
        AssertContainsAny(controller, "[HttpPost]", "HttpPost");
        AssertContainsAny(controller, "CreatedAtAction", "Created(");
        AssertContainsAny(controller, "[HttpGet(\"my\")]", "HttpGet(\"my\")");
        AssertContainsAny(controller, "[HttpGet(\"{id}\")]", "HttpGet(\"{reservationId}\")");
        AssertContainsAny(controller, "[HttpPut(\"{id}/cancel\")]", "cancel");
        AssertContainsAny(controller, "[HttpGet(\"{id}/invoice\")]", "invoice");
        AssertContainsAny(source, "CreateReservationDto", "ReservationDto", "ReservationDetailDto");
        AssertContainsAny(controller, "TryGetCurrentUserId", "ClaimTypes.NameIdentifier", "JwtRegisteredClaimNames.Sub");
        AssertContainsAny(controller, "Unauthorized()", "Unauthorized");
        AssertContainsAny(controller, "Forbid()", "Forbid");
        AssertContainsAny(controller, "NotFound", "bulunamad");
    }

    // S5-06 icin rezervasyon servisinin olusturma ve iptal bug fix kurallarini korudugunu kontrol eder.
    [Fact]
    public void S506_ReservationService_ShouldValidateCreateAndCancelBugFixRules()
    {
        // Rezervasyon API baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReservationApiImplemented())
            return;

        var source = ReadOptionalDomainFile("Services", "ReservationService.cs")
            + Environment.NewLine + ReadOptionalDomainFile("Interfaces", "IReservationRepository.cs")
            + Environment.NewLine + ReadOptionalDomainFile("DTOs", "ReservationDto.cs");

        AssertContainsAny(source, "CreateReservationAsync");
        AssertContainsAny(source, "CheckInDate < DateOnly.FromDateTime", "bugünden önce", "bugunden once");
        AssertContainsAny(source, "CheckInDate >= dto.CheckOutDate", "çıkış tarihinden", "cikis tarihinden");
        AssertContainsAny(source, "GuestCount <= 0", "Misafir sayısı", "Misafir sayisi");
        AssertContainsAny(source, "PaymentMethod", "Ödeme", "Odeme");
        AssertContainsAny(source, "GetRoomWithDetailsAsync", "HotelRoomRepository");
        AssertContainsAny(source, "IsActive", "aktif değil", "aktif degil");
        AssertContainsAny(source, "MaxGuests", "GuestCount");
        AssertContainsAny(source, "MakeReservationAsync");
        AssertContainsAny(source, "CancelReservationAsync");
        AssertContainsAny(source, "reservation.UserId != userId", "!isAdmin");
        AssertContainsAny(source, "Cancelled", "zaten iptal");
        AssertContainsAny(source, "Completed", "Tamamlanmış", "Tamamlanmis");
        AssertContainsAny(source, "reservation.CheckInDate < DateOnly.FromDateTime", "Geçmiş", "Gecmis");
    }

    // S5-06 icin otel detayindan rezervasyon olusturma UI akisinin E23 endpointine guvenli baglandigini kontrol eder.
    [Fact]
    public void S506_ReservationCreateUi_ShouldPostE23FromHotelDetailWithSafeFeedback()
    {
        // Rezervasyon olusturma UI baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReservationCreateUiImplemented())
            return;

        var controller = ReadWebFile("Controllers", "ReservationsController.cs");
        var bookingModal = ReadOptionalWebFile("Views", "Hotels", "_BookingModal.cshtml");
        var roomCard = ReadOptionalWebFile("Views", "Hotels", "_RoomCard.cshtml");
        var source = string.Join(Environment.NewLine, controller, bookingModal, roomCard);

        AssertContainsAny(controller, "AuthFilter", "ServiceFilter", "Authorize", "IsAuthenticated");
        AssertContainsAny(controller, "[HttpPost]", "HttpPost");
        AssertContainsAny(controller, "[ValidateAntiForgeryToken]", "ValidateAntiForgeryToken");
        AssertContainsAny(controller, "PostAsync", "SendAsync");
        AssertContainsAny(controller, "reservations", "api/reservations");
        AssertContainsAny(controller, "TempData[\"Error\"]", "TempData[\"Success\"]", "ModelState");
        AssertContainsAny(controller, "RedirectToAction(\"Detail\", \"Hotels\"", "RedirectToAction(nameof");
        AssertContainsAny(source, "asp-controller=\"Reservations\"", "Reservations");
        AssertContainsAny(source, "asp-action=\"Create\"", "Create");
        AssertContainsAny(source, "HotelId", "HotelRoomId");
        AssertContainsAny(source, "CheckInDate", "CheckOutDate");
        AssertContainsAny(source, "GuestCount", "PaymentMethod");
        AssertContainsAny(source, "min=\"1\"", "MaxGuests", "required");
    }

    // S5-06 icin kullanici rezervasyon sayfalarinin E24-E27 endpointlerine ve bos/hata durumlarina baglandigini kontrol eder.
    [Fact]
    public void S506_ReservationUserPages_ShouldUseMyDetailCancelInvoiceEndpointsWhenStarted()
    {
        // Kullanici rezervasyon sayfalari baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReservationUserPagesImplemented())
            return;

        var controller = ReadWebFile("Controllers", "ReservationsController.cs");
        var myView = ReadOptionalWebFile("Views", "Reservations", "My.cshtml");
        var detailView = ReadOptionalWebFile("Views", "Reservations", "Detail.cshtml");
        var source = string.Join(Environment.NewLine, controller, myView, detailView);

        AssertContainsAny(controller, "AuthFilter", "ServiceFilter", "Authorize", "IsAuthenticated");
        AssertContainsAny(source, "reservations/my", "GetAsync", "GetTAsync");
        AssertContainsAny(source, "reservations/", "Detail", "ReservationId");
        AssertContainsAny(source, "/cancel", "Cancel", "PutAsync");
        AssertContainsAny(source, "/invoice", "Invoice", "Fatura");
        AssertContainsAny(source, "Status", "Cancelled", "Completed", "Pending", "Confirmed");
        AssertContainsAny(source, "disabled", "iptal edilemez", "iptal edilemeyen", "Tamamlanmış", "Tamamlanmis");
        AssertContainsAny(source, "confirm", "Onay", "Emin", "Vazgeç", "Vazgec");
        AssertContainsAny(source, "TempData", "alert", "bos", "boş", "Henüz", "Henuz", "Model.Any");
        Assert.DoesNotContain("PasswordHash", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", source, StringComparison.OrdinalIgnoreCase);
    }

    // S5-06 icin yorum API'sinin E28-E31 endpoint sozlesmesini tasidigini kontrol eder.
    [Fact]
    public void S506_ReviewApi_ShouldExposeE28E29E30E31ContractsWhenStarted()
    {
        // Yorum API baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReviewApiImplemented())
            return;

        var controller = ReadFirstExistingApiFile("Controllers", "ReviewController.cs", "Controllers", "ReviewsController.cs");
        var service = ReadOptionalDomainFile("Services", "ReviewService.cs");
        var dto = ReadOptionalDomainFile("DTOs", "ReviewDto.cs");
        var source = string.Join(Environment.NewLine, controller, service, dto);

        AssertContainsAny(controller, "[Route(\"api/reviews\")]", "api/reviews");
        AssertContainsAny(controller, "[ApiController]", "ControllerBase");
        AssertContainsAny(controller, "HttpGet(\"~/api/hotels/{hotelId}/reviews\")", "api/hotels/{hotelId}/reviews");
        AssertContainsAny(controller, "[HttpPost]", "HttpPost");
        AssertContainsAny(controller, "CreateReviewAsync(userId, dto)", "CreateReviewAsync(userId", "CreateReviewAsync(");
        AssertContainsAny(controller, "Created(", "CreatedAtAction", "StatusCode(201");
        AssertContainsAny(controller, "[HttpDelete(\"{id}\")]", "HttpDelete(\"{reviewId}\")");
        AssertContainsAny(controller, "[HttpGet(\"my\")]", "HttpGet(\"my\")");
        AssertContainsAny(controller, "[Authorize]", "AuthorizeAttribute");
        AssertContainsAny(controller, "Unauthorized()", "Unauthorized");
        AssertContainsAny(source, "CreateReviewDto", "ReviewDto", "HotelId", "Rating", "Comment");
    }

    // S5-06 icin yorum servisinin puan, duplicate yorum ve sahiplik kontrollerini korudugunu kontrol eder.
    // E28'de tamamlanmis rezervasyon sarti opsiyonel oldugu icin bu contract testte zorunlu tutulmaz.
    [Fact]
    public void S506_ReviewService_ShouldValidateRatingDuplicateOwnershipAndMapping()
    {
        // Yorum API baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReviewApiImplemented())
            return;

        var source = ReadOptionalDomainFile("Services", "ReviewService.cs")
            + Environment.NewLine + ReadOptionalDomainFile("Interfaces", "IReviewRepository.cs")
            + Environment.NewLine + ReadOptionalDomainFile("DTOs", "ReviewDto.cs");
        var globalExceptionHandler = ReadOptionalApiFile("Middleware", "GlobalExceptionHandler.cs");

        AssertContainsAny(source, "GetHotelReviewAsync", "GetByHotelIdAsync");
        AssertContainsAny(source, "GetUserReviewsAsync", "GetByUserIdAsync");
        AssertContainsAny(source, "CreateReviewAsync");
        AssertContainsAny(source, "Rating < 1", "Rating > 5", "1 ile 5");
        AssertContainsAny(source, "HotelRepository.GetByIdAsync", "Otel bulunamad");
        AssertContainsAny(source, "GetByUserAndHotelAsync", "daha önce yorum", "daha once yorum");
        AssertContainsAny(source, "InvalidOperationException", "1 ile 5", "daha önce yorum", "daha once yorum");
        Assert.Contains("InvalidOperationException", globalExceptionHandler, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("StatusCodes.Status409Conflict", globalExceptionHandler, StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(source, "DeleteReviewAsync");
        AssertContainsAny(source, "review.UserId != userId", "UnauthorizedAccessException");
        AssertContainsAny(source, "UserName", "HotelName", "CreatedAt");
    }

    // S5-06 icin otel detayindan yorum ekleme UI akisinin E28 endpointine guvenli baglandigini kontrol eder.
    [Fact]
    public void S506_ReviewCreateUi_ShouldPostE28FromHotelDetailWithAuthAndFeedback()
    {
        // Yorum ekleme UI baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReviewCreateUiImplemented())
            return;

        var controller = ReadWebFile("Controllers", "ReviewsController.cs");
        var detailView = ReadOptionalWebFile("Views", "Hotels", "Detail.cshtml");
        var reviewCard = ReadOptionalWebFile("Views", "Hotels", "_ReviewCard.cshtml");
        var source = string.Join(Environment.NewLine, controller, detailView, reviewCard);

        AssertContainsAny(controller, "AuthFilter", "ServiceFilter", "Authorize", "IsAuthenticated");
        AssertContainsAny(controller, "[HttpPost]", "HttpPost");
        AssertContainsAny(controller, "[ValidateAntiForgeryToken]", "ValidateAntiForgeryToken");
        AssertContainsAny(controller, "PostAsync", "SendAsync");
        AssertContainsAny(controller, "reviews", "api/reviews");
        AssertContainsAny(controller, "hotelId");
        AssertContainsAny(controller, "model.HotelId = hotelId", "HotelId = hotelId");
        AssertContainsAny(controller, "TempData[\"Error\"]", "TempData[\"Success\"]", "ModelState");
        AssertContainsAny(controller, "RedirectToAction(\"Detail\", \"Hotels\"", "RedirectToAction(nameof");
        AssertContainsAny(source, "asp-controller=\"Reviews\"", "Reviews");
        AssertContainsAny(source, "asp-action=\"Create\"", "Create");
        AssertContainsAny(source, "Rating", "Puan");
        AssertContainsAny(source, "Comment", "Yorum");
        AssertContainsAny(source, "AntiForgeryToken", "asp-antiforgery", "method=\"post\"");
    }

    // S5-06 icin kullanici yorum sayfalarinin E30-E31 endpointlerine ve bos/hata durumlarina baglandigini kontrol eder.
    [Fact]
    public void S506_ReviewUserPages_ShouldUseMyAndDeleteEndpointsWhenStarted()
    {
        // Kullanici yorum sayfalari baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReviewUserPagesImplemented())
            return;

        var controller = ReadWebFile("Controllers", "ReviewsController.cs");
        var myView = ReadOptionalWebFile("Views", "Reviews", "My.cshtml");
        var loginPartial = ReadOptionalWebFile("Views", "Shared", "_LoginPartial.cshtml");
        var source = string.Join(Environment.NewLine, controller, myView, loginPartial);

        AssertContainsAny(controller, "AuthFilter", "ServiceFilter", "Authorize", "IsAuthenticated");
        AssertContainsAny(source, "reviews/my", "GetAsync", "GetTAsync");
        AssertContainsAny(source, "DeleteAsync", "reviews/", "Delete");
        AssertContainsAny(source, "[ValidateAntiForgeryToken]", "AntiForgeryToken", "asp-antiforgery");
        AssertContainsAny(source, "confirm", "Onay", "Emin", "Silmek");
        AssertContainsAny(source, "TempData", "alert", "bos", "boş", "Henüz", "Henuz", "Model.Any");
        AssertContainsAny(source, "asp-controller=\"Reviews\"", "Reviews/My", "Yorumlarım", "Yorumlarim");
        Assert.DoesNotContain("PasswordHash", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", source, StringComparison.OrdinalIgnoreCase);
    }

    // S5-06 icin otel detay sayfasinda bos rezervasyon/yorum ve login yonlendirme durumlarinin sayfayi bozmadigini kontrol eder.
    [Fact]
    public void S506_HotelDetail_ShouldRenderEmptyAndUnauthorizedStatesSafelyWhenFlowStarted()
    {
        // Rezervasyon veya yorum akisi baslamadiysa CI passed gecer.
        if (!Sprint5CardGuards.S506_ReservationReviewFlowImplemented())
            return;

        var detailView = ReadWebFile("Views", "Hotels", "Detail.cshtml");
        var bookingModal = ReadOptionalWebFile("Views", "Hotels", "_BookingModal.cshtml");
        var reviewCard = ReadOptionalWebFile("Views", "Hotels", "_ReviewCard.cshtml");
        var source = string.Join(Environment.NewLine, detailView, bookingModal, reviewCard);

        AssertContainsAny(source, "IsAuthenticated", "Auth", "Login");
        AssertContainsAny(source, "Rezervasyon", "Booking", "Uygun odalar", "Uygun Odaları");
        AssertContainsAny(source, "Yorumlar", "Reviews", "_ReviewCard");
        AssertContainsAny(source, "henüz", "henuz", "bulunmuyor", "paylaşılmamış", "paylasilmamis", "Model.Reviews != null");
        AssertContainsAny(source, "TempData[\"Error\"]", "alert-danger", "alert-info", "alert");
        Assert.DoesNotContain("PasswordHash", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ConnectionString", source, StringComparison.OrdinalIgnoreCase);
    }

    // API projesindeki opsiyonel bir dosyayi varsa okur.
    private static string ReadOptionalApiFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.API") }.Concat(relativePathParts).ToArray());

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    // API projesindeki iki olasi yoldan ilk bulunan dosyayi okur.
    private static string ReadFirstExistingApiFile(params string[] pathParts)
    {
        Assert.True(pathParts.Length % 2 == 0, "Dosya yolu parcalari ikili verilmelidir.");

        for (var i = 0; i < pathParts.Length; i += 2)
        {
            var path = Path.Combine(FindProjectRoot("HotelApp.API"), pathParts[i], pathParts[i + 1]);
            if (File.Exists(path))
                return File.ReadAllText(path);
        }

        throw new FileNotFoundException("Beklenen API dosyalarindan hicbiri bulunamadi.");
    }

    // Domain projesindeki opsiyonel bir dosyayi varsa okur.
    private static string ReadOptionalDomainFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.Domain") }.Concat(relativePathParts).ToArray());

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
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

    // Metin icinde beklenen parcalardan en az birinin gectigini kontrol eder.
    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase)),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }
}
