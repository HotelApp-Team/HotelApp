using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    public class HotelListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public byte StarRating { get; set; }
        public decimal? AvgRating { get; set; }
        public bool IsActive { get; set; }
        public string? MainImageUrl { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
