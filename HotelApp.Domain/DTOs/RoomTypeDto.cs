using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Oda tipi bilgilerini içeren Dto
    public class RoomTypeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Oda tipi adı zorunludur.")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter olabilir.")]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Lütfen kişi sayısını giriniz.")]
        public int MaxGuests { get; set; }
    }
    // Oda tipi oluşturmak için Dto
    public class CreateRoomTypeDto
    {
        [Required(ErrorMessage = "Oda tipi adı zorunludur.")]
        [StringLength(100, ErrorMessage = "En fazla 100 kaarakter olabilir.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Oda tipi açıklaması zornludur.")]
        [StringLength(500, ErrorMessage = "En fazla 500 karakter olabilir.")]
        public string Description { get; set; } = null!;
        [Range(1, 20, ErrorMessage ="Lütfen geçerli bir kişi sayısı giriniz.")]
        public int MaxGuests { get; set; }
    }
    // Oda tipi güncellemek için Dto
    public class UpdateRoomTypeDto
    {
        [Range(1, int.MaxValue, ErrorMessage ="Lütfen geçerli bir Id giriniz.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Oda tipi adı zorunludur.")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter olabilir.")]

        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(500, ErrorMessage = "En fazla 500 karakter olabilir.")]
        public string Description { get; set; } = null!;

        [Range(1,20,ErrorMessage ="Kişi sayısı 1 ile 20 arasında olmalıdır.")]
        public int MaxGuests { get; set; }
    }
}