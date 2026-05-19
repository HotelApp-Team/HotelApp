using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

public interface IRoomAmenityRepository : IRepository<RoomAmenity>
{
    Task<RoomAmenity?> GetByNameAsync(string name);
    Task<List<RoomAmenity>> GetByHotelRoomIdAsync(int hotelRoomId);
}
