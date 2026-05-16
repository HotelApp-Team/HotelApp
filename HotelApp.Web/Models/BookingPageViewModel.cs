namespace HotelApp.Web.Models
{
    public class BookingPageViewModel
    {
        public int HotelId { get; set; }
        public int HotelRoomId { get; set; }

        public string HotelName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;

        public decimal PricePerNight { get; set; }
        public decimal ExtraGuestPricePerNight { get; set; }
        public int MaxGuests { get; set; }

        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int GuestCount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
