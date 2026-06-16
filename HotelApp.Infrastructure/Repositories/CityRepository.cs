using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;

namespace HotelApp.Infrastructure.Repositories;

// City CRUD'u jenerik Repository'den miras alir.
// Sehirler seed-only oldugu icin ek metoda gerek yoktur.
public class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(HotelAppDbContext context) : base(context)
    {
    }
}
