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
        public int RoomId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public bool IsActive { get; set; }
        public int Available { get; set; }
    }
}
