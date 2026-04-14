using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.DTOs
{
    // Oda bilgilerini içeren Dto
    public class HotelRoomDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Oda tipi zorunludur.")]
        public int RoomTypeId { get; set; }

        [Range(0.01,double.MaxValue,ErrorMessage ="Fiyat 0'dan büyük olmalı.")]
        public decimal PricePerNight { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Oda sayısı 0'dan büyük olmalı.")]

        public int TotalRooms { get; set; } 

        public string HotelName { get; set; } = null!;
        public string RoomTypeName { get; set; } = null!;
    }

    // Detaylı bilgi içeren Dto
    public class DetailHotelRoomDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public int TotalRooms { get; set; }
        public bool IsActive { get; set; }
        public string HotelName { get; set; }
        public string RoomTypeName { get; set; } = null!;

        public int ReservationCount { get; set; }
    }

    // Oluşturma İşlemi için Dto
    public class CreateHotelRoomDto
    {
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public int TotalRooms { get; set; }
    }

    // Güncelleme İşlemi için Dto
    public class UpdateHotelRoomDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public int TotalRooms { get; set; }
        public bool IsActive { get; set; }
    }

}
