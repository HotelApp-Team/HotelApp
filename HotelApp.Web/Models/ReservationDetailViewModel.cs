namespace HotelApp.Web.Models
{
    public class ReservationDetailViewModel
    {
        public int ReservationId { get; set; }
        // Otel detay sayfasinda "Yorum Yaz" form action'i HotelId'ye ihtiyac duyar;
        // API DTO uzerinden tasiniyor (JSON deserialization otomatik map eder).
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int GuestCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
