using System;
using System.Collections.Generic;

namespace HotelApp.Domain.Entities;

// Oda manzara turleri (Deniz, Sehir, Dag vb.) icin lookup entity'si.
// Scaffold ile uretilen partial class ile uyumlu yapida; admin tarafinda
// sadece secim listesi olarak kullanilir, CRUD yok.
public partial class RoomView
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<HotelRoom> HotelRooms { get; set; } = new List<HotelRoom>();
}
