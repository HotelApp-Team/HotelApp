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

    // Otele ait tüm görsellerin sıralama ve ana görsel ayarlarını tek istekte günceller.
    public class HotelImageUpdateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "En az bir görsel gönderilmelidir")]
        public List<HotelImageUpdateItemDto> Images { get; set; } = new();
    }

    // Toplu güncelleme isteğindeki tek bir görselin kimlik ve ayar bilgilerini taşır.
    public class HotelImageUpdateItemDto
    {
        public int Id { get; set; }

        public bool IsMain { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "DisplayOrder 0 veya daha büyük olmalıdır")]
        public int DisplayOrder { get; set; }
    }
}
