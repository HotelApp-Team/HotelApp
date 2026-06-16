using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Web.Models;

namespace HotelApp.Tests;

// Teslim sonrasi rezervasyon karti ve yorum onay akisi sozlesmelerini korur.
// SOLID: SRP kullanir; sadece bu iyilestirmelerin veri ve UI sozlesmelerini dogrular.
public class PostDeliveryImprovementContractTests
{
    [Fact]
    public void ReservationModels_ShouldCarryMainHotelImage()
    {
        Assert.NotNull(typeof(VwReservationDetail).GetProperty("MainImageUrl"));
        Assert.NotNull(typeof(ReservationDetailDto).GetProperty("MainImageUrl"));
        Assert.NotNull(typeof(ReservationDetailViewModel).GetProperty("MainImageUrl"));
    }

    [Fact]
    public void ReviewModels_ShouldCarryApprovalState()
    {
        Assert.NotNull(typeof(Review).GetProperty("IsApproved"));
        Assert.NotNull(typeof(Review).GetProperty("ApprovedAt"));
        Assert.NotNull(typeof(ReviewDto).GetProperty("IsApproved"));
        Assert.NotNull(typeof(AdminReviewViewModel).GetProperty("IsApproved"));
    }

    [Fact]
    public void PublicReviewQueries_ShouldOnlyReturnApprovedReviews()
    {
        var repository = ReadProjectFile(
            "HotelApp.Infrastructure", "Repositories", "ReviewRepository.cs");

        Assert.Contains("x.HotelId == hotelId && x.IsApproved", repository);
        Assert.Contains(".Where(x => x.IsApproved)", repository);
    }

    [Fact]
    public void AdminAndReservationViews_ShouldExposeNewBehavior()
    {
        var adminView = ReadProjectFile(
            "HotelApp.Web", "Views", "AdminReviews", "Index.cshtml");
        var reservationView = ReadProjectFile(
            "HotelApp.Web", "Views", "Reservations", "My.cshtml");

        Assert.Contains("asp-action=\"Approve\"", adminView);
        Assert.Contains("Onay Bekliyor", adminView);
        Assert.Contains("MainImageUrl", reservationView);
        Assert.Contains("ToString(\"N2\", trCulture) TL", reservationView);
    }

    private static string ReadProjectFile(params string[] segments)
    {
        var projectRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var path = Path.Combine(new[] { projectRoot }.Concat(segments).ToArray());

        return File.ReadAllText(path);
    }
}
