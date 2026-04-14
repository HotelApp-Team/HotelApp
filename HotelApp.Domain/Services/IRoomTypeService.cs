using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IRoomTypeService
    {
        Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync();
        Task<RoomTypeDto?> GetRoomTypeByIdAsync(int id);
        Task<RoomTypeDto> GetRoomTypeByNameAsync(string name);
        Task<IEnumerable<RoomTypeDto>> GetRoomTypeByMaxGuestsAsync(int maxGuests);

        Task AddRoomTypeAsync(RoomTypeDto dto);
        Task UpdateRoomTypeAsync(RoomTypeDto dto);
        Task DeleteRoomTypeAsync(int id);
    }
}
