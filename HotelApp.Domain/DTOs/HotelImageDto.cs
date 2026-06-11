using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Otel resim bilgilerini listelemek için kullanılır.
    // ImageUrl görsel adresini, IsMain ana görseli, DisplayOrder ise sıralamayı belirtir.
    public class HotelImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Yeni otel resmi eklemek için kullanılır.
    // Otel detay veya admin ekranından gönderilen görsel bilgilerini taşır.
    public class HotelImageCreateDto
    {
        [Required(ErrorMessage = "Resim URL zorunludur")]
        public string ImageUrl { get; set; } = null!;

        public bool IsMain { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "DisplayOrder 0 veya daha büyük olmalıdır")]
        public int DisplayOrder { get; set; }
    }

    // Mevcut otel resmini güncellemek için kullanılır.
    // Id ile hangi görselin güncelleneceği belirlenir.
    public class HotelImageUpdateDto
    {
        [Required(ErrorMessage = "Id zorunludur")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Resim URL zorunludur")]
        public string ImageUrl { get; set; } = null!;

        public bool IsMain { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "DisplayOrder 0 veya daha büyük olmalıdır")]
        public int DisplayOrder { get; set; }
    }
}
