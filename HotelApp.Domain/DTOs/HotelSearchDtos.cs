using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Search Request DTO
    public class HotelSearchRequestDto
    {
        public string? City { get; set; }
        public byte? StarRating { get; set; }
        public decimal? MinRating { get; set; }

        // Availability search için (ileride lazım olacak)
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? GuestCount { get; set; }
    }

    // Search Response DTO
    public class HotelSearchResponseDto
    {
        public int TotalCount { get; set; }
        public List<HotelListDto> Hotels { get; set; } = new();
    }
}
