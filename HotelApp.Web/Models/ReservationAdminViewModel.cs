namespace HotelApp.Web.Models
{
    // Admin rezervasyon yönetimi ekranında her bir rezervasyon satırını göstermek için kullanılır.
    public class ReservationAdminViewModel
    {
        public int Id { get; set; }
        public int HotelRoomId { get; set; }
        public int UserId { get; set; }

        public string HotelName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int GuestCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
