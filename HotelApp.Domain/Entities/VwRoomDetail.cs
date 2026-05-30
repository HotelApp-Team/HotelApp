using System;
using System.Collections.Generic;

namespace HotelApp.Domain.Entities;

public partial class VwRoomDetail
{
    public int HotelRoomId { get; set; }

    public int HotelId { get; set; }

    public string HotelName { get; set; } = null!;

    public string City { get; set; } = null!;

    public string RoomTypeName { get; set; } = null!;

    public int MaxGuests { get; set; }

    public decimal PricePerNight { get; set; }

    public decimal ExtraGuestPricePerNight { get; set; }

    public int TotalRooms { get; set; }

    public bool IsActive { get; set; }

    // Manzara alanlari (vw_RoomDetails view'inden LEFT JOIN ile gelir).
    // NULL olabilir; eski kayitlarda veya manzara secilmemis odalarda bos.
    public int? RoomViewId { get; set; }

    public string? RoomViewName { get; set; }

    public string? RoomViewIcon { get; set; }
}
