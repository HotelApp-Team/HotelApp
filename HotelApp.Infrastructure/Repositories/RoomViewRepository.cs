using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;

namespace HotelApp.Infrastructure.Repositories;

// RoomView CRUD'u jenerik Repository'den miras alir.
// Manzara turleri seed-only oldugu icin ek metoda gerek yok.
public class RoomViewRepository : Repository<RoomView>, IRoomViewRepository
{
    public RoomViewRepository(HotelAppDbContext context) : base(context)
    {
    }
}
