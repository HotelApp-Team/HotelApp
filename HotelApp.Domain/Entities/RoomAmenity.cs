using System;
using System.Collections.Generic;

namespace HotelApp.Domain.Entities;

public partial class RoomAmenity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public virtual ICollection<HotelRoom> HotelRooms { get; set; } = new List<HotelRoom>();
}
