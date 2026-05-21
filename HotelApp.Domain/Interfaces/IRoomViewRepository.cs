using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces
{
    // RoomView icin repository sozlesmesi.
    // Generic IRepository CRUD'unu miras alir; ek metoda gerek yok cunku
    // manzara turleri seed-only, sadece liste sorgusu kullanilir.
    public interface IRoomViewRepository : IRepository<RoomView>
    {
    }
}
