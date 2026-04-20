using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    public interface IHotelRoomRepository : IRepository<HotelRoom>
    {
        //Oda ilişkile veriler 
        Task<HotelRoom?> GetRoomWithDetailsAsync(int id);
        //Tüm odalar ve ilişkili veriler
        Task<IEnumerable<HotelRoom>> GetRoomsWithDetailsAsync();
        //Belirli bir otele ait odalar
        Task<IEnumerable<HotelRoom>> GetRoomsByHotelAsync(int hotelId);
        //Belirli bir oda tipine sahip odalar
        Task<IEnumerable<HotelRoom>> GetRoomsByRoomTypeAsync(int roomTypeId);

        Task<IEnumerable<HotelRoom>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);

    }
}
