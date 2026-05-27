namespace HotelApp.Web.Models
{
    public class AvailableRoomViewModel
    {
        public int Id { get; set; }
        public int HotelRoomId { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public decimal ExtraGuestPricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int TotalRooms { get; set; }
        public int ReservedCount { get; set; }
        public int AvailableRooms { get; set; }

        public bool IsActive { get; set; }
        public List<RoomAmenityViewModel> RoomAmenities { get; set; } = new();

        public string? MainImageUrl { get; set; }

        public List<RoomImageViewModel> Images { get; set; } = new();

    }
}
