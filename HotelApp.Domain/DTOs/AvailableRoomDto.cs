using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Kullanıcıya gösterilecek müsait odaların bilgilerini içeren Dto
    public class AvailableRoomDto
    {
        public int HotelRoomId { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int TotalRooms { get; set; }
        public int ReservedCount { get; set; }
        public int AvailableRooms { get; set; }
    }
}
