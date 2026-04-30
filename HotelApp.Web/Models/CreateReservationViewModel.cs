namespace HotelApp.Web.Models
{
    public class CreateReservationViewModel
    {
        public int HotelId { get; set; }
        public int HotelRoomId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int GuestCount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
