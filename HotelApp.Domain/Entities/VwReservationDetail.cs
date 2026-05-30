using System;
using System.Collections.Generic;

namespace HotelApp.Domain.Entities;

public partial class VwReservationDetail
{
    public int ReservationId { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public int GuestCount { get; set; }

    public decimal TotalPrice { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int HotelId { get; set; }

    public string HotelName { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public int RoomTypeId { get; set; }

    public string RoomTypeName { get; set; } = null!;

    public int MaxGuests { get; set; }

    public decimal PricePerNight { get; set; }
}
