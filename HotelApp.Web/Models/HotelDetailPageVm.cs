namespace HotelApp.Web.Models
{
    public class HotelDetailPageVm
    {
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal? AvgRating { get; set; }

        public List<AvailableRoomViewModel> Rooms { get; set; } = new();
        public List<ReviewViewModel> Reviews { get; set; } = new();

        public CreateReservationViewModel BookingForm { get; set; } = new();

        public bool IsAuthenticated { get; set; }
    }
}
