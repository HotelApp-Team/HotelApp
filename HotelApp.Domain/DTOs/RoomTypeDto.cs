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
        [Required(ErrorMessage="Oda tipi adı zorunludur.")]
        [StringLength(100,ErrorMessage ="En fazla 100 karakter olabilir.")]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        [Range(1, int.MaxValue, ErrorMessage ="Lütfen kişi saysını giriniz.")]
        public int MaxGuests { get; set; }  
        public string HotelRoomCount { get; set; } = null!;
    }
    // Oda tipi oluşturmak için Dto
    public class CreateRoomTypeDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int MaxGuests { get; set; }
    }
    // Oda tipi güncellemek için Dto
    public class UpdateRoomTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int MaxGuests { get; set; }
    }
}