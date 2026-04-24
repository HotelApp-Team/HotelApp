using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class AmenityCreateDto
    {
        [Required(ErrorMessage = "Olanak adı zorunludur")]
        public string Name { get; set; } = null!;

        public string? Icon { get; set; }
    }

    // Otele amenity ekleme/kaldırma işlemleri için DTO
    public class AmenityUpdateDto
    {
        [Required(ErrorMessage = "Id zorunludur")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Olanak adı zorunludur")]
        public string Name { get; set; } = null!;

        public string? Icon { get; set; }
    }

    // Otel detayında amenity listelemek için
    public class HotelAmenityAssignDto
    {
        [Required(ErrorMessage = "AmenityId listesi zorunludur")]
        public List<int> AmenityIds { get; set; } = new();
    }
    public class HotelAmenityAssignResponseDto
    {
        public int AddedCount { get; set; }
        public int NotFoundCount { get; set; }
        public List<int> NotFoundIds { get; set; } = new();
    }
}
