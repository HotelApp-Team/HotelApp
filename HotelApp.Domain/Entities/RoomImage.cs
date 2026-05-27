using System;
using System.Collections.Generic;

namespace HotelApp.Domain.Entities;

public partial class RoomImage
{
    public int Id { get; set; }

    public int HotelRoomId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsMain { get; set; }

    public int DisplayOrder { get; set; }

    public virtual HotelRoom HotelRoom { get; set; } = null!;
}
