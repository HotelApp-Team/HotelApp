using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    public interface IRoomTypeRepository : IRepository<RoomType>
    {
        Task<RoomType?> GetRoomTypeWithRoomsAsync(int id);
        Task<IEnumerable<RoomType>> GetAllWithRoomsAsync();
        Task<IEnumerable<RoomType>> GetByMaxGuestsAsync(int maxGuests);
        Task<RoomType?> GetByNameAsync(string name);

    }
}
