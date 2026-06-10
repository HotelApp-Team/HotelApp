using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Mevcut otel bilgilerini güncellemek için API'ye gönderilen verileri taşır.
    // IsActive alanı ile otelin aktif veya pasif durumu da güncellenebilir.
    public class HotelUpdateDto
    {
        [Required(ErrorMessage = "Id zorunludur")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Otel adı zorunludur")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Adres zorunludur")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Şehir zorunludur")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Ülke zorunludur")]
        public string Country { get; set; } = null!;

        [Required(ErrorMessage = "Telefon zorunludur")]
        public string Phone { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz")]
        public string? Email { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [Range(1, 5, ErrorMessage = "Yıldız sayısı 1 ile 5 arasında olmalıdır")]
        public byte StarRating { get; set; }

        public bool IsActive { get; set; }
    }
}
