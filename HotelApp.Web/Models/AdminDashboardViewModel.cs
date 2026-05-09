namespace HotelApp.Web.Models;

public class AdminDashboardViewModel
{
    public int TotalHotels { get; set; }
    public int TotalUsers { get; set; }
    public int TotalReservations { get; set; }
    public int ActiveReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
    public List<RecentReservationViewModel>? RecentReservations { get; set; }
}
