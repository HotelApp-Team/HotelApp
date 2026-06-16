using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Yeni otel oluşturma işlemi için API'ye gönderilen verileri taşır.
    // Required ve Range validasyonları ile zorunlu alanları kontrol eder.
    public class HotelCreateDto
    {
        [Required(ErrorMessage = "Otel adı zorunludur")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Adres zorunludur")]
        public string Address { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Şehir zorunludur")]
        public int CityId { get; set; }

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
    }
}
