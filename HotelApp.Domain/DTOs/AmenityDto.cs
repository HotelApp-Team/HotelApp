using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Otel olanaklarını listelemek için kullanılan DTO'dur.
    // Örneğin Wi-Fi, otopark, havuz gibi bilgileri taşır.
    public class AmenityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
    }

    // Yeni olanak oluşturmak için API'ye gönderilen verileri taşır.
    // Name zorunludur, Icon isteğe bağlıdır.
    public class AmenityCreateDto
    {
        [Required(ErrorMessage = "Olanak adı zorunludur")]
        public string Name { get; set; } = null!;

        public string? Icon { get; set; }
    }

    // Mevcut olanak bilgisini güncellemek için kullanılır.
    // Id güncellenecek kaydı, Name ve Icon ise yeni değerleri temsil eder.
    public class AmenityUpdateDto
    {
        [Required(ErrorMessage = "Id zorunludur")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Olanak adı zorunludur")]
        public string Name { get; set; } = null!;

        public string? Icon { get; set; }
    }

    // Bir otele birden fazla olanak atamak için kullanılır.
    // AmenityIds listesi seçilen olanakların Id değerlerini taşır.
    public class HotelAmenityAssignDto
    {
        [Required(ErrorMessage = "AmenityId listesi zorunludur")]
        public List<int> AmenityIds { get; set; } = new();
    }

    // Otel olanak atama işleminden sonra sonucu döndürmek için kullanılır.
    // Kaç olanak eklendiğini ve bulunamayan Id listesini taşır.
    public class HotelAmenityAssignResponseDto
    {
        public int AddedCount { get; set; }
        public int NotFoundCount { get; set; }
        public List<int> NotFoundIds { get; set; } = new();
    }
}
