using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Olanak listeleme DTO'su
    public class AmenityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
    }

    // Olanak güncelleme DTO'su
    public class CreateAmenityDto
    {
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
    }

    // Otele amenity ekleme/kaldırma işlemleri için DTO
    public class UpdateAmenityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
    }

    // Otel detayında amenity listelemek için
    public class HotelAmenityAssignDto
    {
        public int HotelId { get; set; }
        public int AmenityId { get; set; }
    }
}
