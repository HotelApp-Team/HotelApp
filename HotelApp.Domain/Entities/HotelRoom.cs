using System;
using System.Collections.Generic;

namespace HotelApp.Domain.Entities;

public partial class HotelRoom
{
    public int Id { get; set; }

    public int HotelId { get; set; }

    public int RoomTypeId { get; set; }

    public decimal PricePerNight { get; set; }

    public decimal ExtraGuestPricePerNight { get; set; }

    public int TotalRooms { get; set; }

    public bool IsActive { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();

    public virtual RoomType RoomType { get; set; } = null!;
}
