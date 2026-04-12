using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Otel resim bilgilerini listelemek için kullanılır
    public class HotelImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Yeni otel resmi eklemek için kullanılır
    public class HotelImageCreateDto
    {
        public int HotelId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Mevcut otel resmini güncellemek için kullanılır
    public class HotelImageUpdateDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Otel + tüm resimlerini birlikte döndürmek için kullanılır
    public class HotelWithImagesDto
    {
        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;

        public List<HotelImageDto> Images { get; set; } = new();
    }
}
