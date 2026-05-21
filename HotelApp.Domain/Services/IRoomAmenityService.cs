using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

public interface IRoomAmenityService
{
    Task<List<RoomAmenityDto>> GetAllAsync();
    Task<RoomAmenityDto?> GetByIdAsync(int id);
    Task AddAsync(RoomAmenityCreateDto dto);
    Task UpdateAsync(RoomAmenityUpdateDto dto);
    Task DeleteAsync(int id);
    Task<List<RoomAmenityDto>> GetByHotelRoomAsync(int hotelId, int roomId);
    Task<List<RoomAmenityDto>> SetHotelRoomAmenitiesAsync(int hotelId, int roomId, List<int> roomAmenityIds);
    Task RemoveFromHotelRoomAsync(int hotelId, int roomId, int roomAmenityId);
}
