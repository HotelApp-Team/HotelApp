using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IHotelRoomService
    {
        Task<IEnumerable<HotelRoomDto>> GetAllRoomsAsync();
        Task<DetailHotelRoomDto?> GetRoomByIdAsync(int id);
        Task<IEnumerable<HotelRoomDto>> GetRoomsByHotelAsync(int hotelId);
        Task<IEnumerable<HotelRoomDto>> GetRoomsByTypeAsync(int roomTypeId);
        Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);

        Task AddRoomsAsync(CreateHotelRoomDto dto);
        Task UpdateRoomsAsync(UpdateHotelRoomDto dto);
        Task DeleteRoomsAsync(int id);
    }
}
