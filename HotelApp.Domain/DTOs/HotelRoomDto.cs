using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Oda bilgilerini içeren Dto
namespace HotelApp.Domain.DTOs
{
    public class HotelRoomDto
    {
        public int Id { get; set; }

        public int HotelId { get; set; }

        public int RoomTypeId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalı.")]
        public decimal PricePerNight { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ek kişi ücreti 0 veya daha büyük olmalı.")]
        public decimal ExtraGuestPricePerNight { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Oda sayısı en az 1 olmalı.")]
        public int TotalRooms { get; set; }
        public bool IsActive { get; set; }
        public int MaxGuests { get; set; }


        public string HotelName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
    }

    public class DetailHotelRoomDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal ExtraGuestPricePerNight { get; set; }
        public int TotalRooms { get; set; }
        public bool IsActive { get; set; }
        public string HotelName { get; set; } = string.Empty;  // düzeltildi
        public string RoomTypeName { get; set; } = string.Empty;
        public int ReservationCount { get; set; }
    }

    public class CreateHotelRoomDto
    {
        [Required(ErrorMessage = "HotelId zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir HotelId giriniz.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "RoomTypeId zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir RoomTypeId giriniz.")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalı.")]
        public decimal PricePerNight { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ek kişi ücreti 0 veya daha büyük olmalı.")]
        public decimal ExtraGuestPricePerNight { get; set; }

        [Required(ErrorMessage = "Oda sayısı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Oda sayısı en az 1 olmalı.")]
        public int TotalRooms { get; set; }
    }

    public class CreateHotelRoomRequestDto
    {
        [Required(ErrorMessage = "RoomTypeId zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir RoomTypeId giriniz.")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalı.")]
        public decimal PricePerNight { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ek kişi ücreti 0 veya daha büyük olmalı.")]
        public decimal ExtraGuestPricePerNight { get; set; }

        [Required(ErrorMessage = "Oda sayısı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Oda sayısı en az 1 olmalı.")]
        public int TotalRooms { get; set; }
    }

    public class UpdateHotelRoomDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "HotelId zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir HotelId giriniz.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "RoomTypeId zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir RoomTypeId giriniz.")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalı.")]
        public decimal PricePerNight { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ek kişi ücreti 0 veya daha büyük olmalı.")]
        public decimal ExtraGuestPricePerNight { get; set; }

        [Required(ErrorMessage = "Oda sayısı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Oda sayısı en az 1 olmalı.")]
        public int TotalRooms { get; set; }

        public bool IsActive { get; set; }
    }

    public class UpdateHotelRoomRequestDto
    {
        [Required(ErrorMessage = "RoomTypeId zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir RoomTypeId giriniz.")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalı.")]
        public decimal PricePerNight { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ek kişi ücreti 0 veya daha büyük olmalı.")]
        public decimal ExtraGuestPricePerNight { get; set; }

        [Required(ErrorMessage = "Oda sayısı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Oda sayısı en az 1 olmalı.")]
        public int TotalRooms { get; set; }

        public bool IsActive { get; set; }
    }

}

