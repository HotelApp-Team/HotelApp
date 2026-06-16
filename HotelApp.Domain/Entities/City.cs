using System;
using System.Collections.Generic;

namespace HotelApp.Domain.Entities;

// Sehir (il) lookup entity'si. Hotels.CityId bu tabloya FK ile baglidir.
// Scaffold ile uretilen partial class yapisiyla uyumludur.
public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
