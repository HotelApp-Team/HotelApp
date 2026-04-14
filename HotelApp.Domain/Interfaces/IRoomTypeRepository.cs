using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    public interface IRoomTypeRepository
    {
        Task<RoomType?> GetByIdAsync(int id);
        Task<IEnumerable<RoomType>> GetAllAsync();
        Task<RoomType?> GetRoomTypeWithRoomsAsync(int id);
        Task<IEnumerable<RoomType>> GetAllWithRoomsAsync();
        Task<IEnumerable<RoomType>> GetByMaxGuestsAsync(int maxGuests);
        Task<RoomType?> GetByNameAsync(string name);

        Task AddAsync(RoomType entity);
        Task UpdateAsync(RoomType entity);
        Task DeleteAsync(int id);
        Task AddRoomTypeAsync(RoomType entity);
    }
}
