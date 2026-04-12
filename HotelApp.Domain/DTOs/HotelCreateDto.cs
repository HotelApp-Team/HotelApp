using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    public class HotelCreateDto
    {
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
    }
}
