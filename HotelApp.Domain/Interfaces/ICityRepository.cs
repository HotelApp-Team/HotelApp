using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces
{
    // City (sehir lookup) icin repository sozlesmesi.
    // Generic IRepository CRUD'unu miras alir; sehirler seed-only oldugu icin
    // ek metoda gerek yoktur, sadece liste sorgusu kullanilir.
    public interface ICityRepository : IRepository<City>
    {
    }
}
