using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    public class HotelDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;

        public string Phone { get; set; } = null!;
        public string? Email { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public byte StarRating { get; set; }
        public decimal? AvgRating { get; set; }

        public bool IsActive { get; set; }

        public List<AmenityDto> Amenities { get; set; } = new();
    }
}
