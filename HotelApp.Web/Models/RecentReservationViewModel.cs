namespace HotelApp.Web.Models;

public class RecentReservationViewModel
{
    public int Id { get; set; }
    public string? HotelName { get; set; }
    public string? UserName { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string? Status { get; set; }
    public decimal TotalPrice { get; set; }

}
