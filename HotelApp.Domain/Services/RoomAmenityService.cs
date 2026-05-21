using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services;

public class RoomAmenityService : IRoomAmenityService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomAmenityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<RoomAmenityDto>> GetAllAsync()
    {
        var amenities = await _unitOfWork.RoomAmenityRepository.GetAllAsync();
        return amenities.Select(MapToDto).ToList();
    }

    public async Task<RoomAmenityDto?> GetByIdAsync(int id)
    {
        var amenity = await _unitOfWork.RoomAmenityRepository.GetByIdAsync(id);
        return amenity == null ? null : MapToDto(amenity);
    }

    public async Task AddAsync(RoomAmenityCreateDto dto)
    {
        var name = NormalizeName(dto.Name);
        var existing = await _unitOfWork.RoomAmenityRepository.GetByNameAsync(name);

        if (existing != null)
            throw new InvalidOperationException("Bu oda olanağı zaten mevcut.");

        var entity = new RoomAmenity
        {
            Name = name,
            Icon = NormalizeIcon(dto.Icon)
        };

        await _unitOfWork.RoomAmenityRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoomAmenityUpdateDto dto)
    {
        var entity = await _unitOfWork.RoomAmenityRepository.GetByIdAsync(dto.Id);
        if (entity == null)
            throw new KeyNotFoundException("Oda olanağı bulunamadı.");

        var name = NormalizeName(dto.Name);
        var duplicate = await _unitOfWork.RoomAmenityRepository
            .ExistsAsync(x => x.Id != dto.Id && x.Name.ToLower() == name.ToLower());

        if (duplicate)
            throw new InvalidOperationException("Bu isimde bir oda olanağı zaten var.");

        entity.Name = name;
        entity.Icon = NormalizeIcon(dto.Icon);

        _unitOfWork.RoomAmenityRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var amenity = await _unitOfWork.RoomAmenityRepository.GetByIdAsync(id);
        if (amenity == null)
            throw new KeyNotFoundException("Oda olanağı bulunamadı.");

        var isUsed = await _unitOfWork.HotelRoomRepository
            .ExistsAsync(x => x.RoomAmenities.Any(a => a.Id == id));

        if (isUsed)
            throw new InvalidOperationException("Bu oda olanağı odalar tarafından kullanılıyor, silinemez.");

        _unitOfWork.RoomAmenityRepository.Delete(amenity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<RoomAmenityDto>> GetByHotelRoomAsync(int hotelId, int roomId)
    {
        var room = await GetOwnedHotelRoomAsync(hotelId, roomId);
        return room.RoomAmenities.Select(MapToDto).ToList();
    }

    public async Task<List<RoomAmenityDto>> SetHotelRoomAmenitiesAsync(int hotelId, int roomId, List<int> roomAmenityIds)
    {
        var room = await GetOwnedHotelRoomAsync(hotelId, roomId);
        var selectedIds = roomAmenityIds.Distinct().ToList();
        var selectedAmenities = new List<RoomAmenity>();

        foreach (var amenityId in selectedIds)
        {
            var amenity = await _unitOfWork.RoomAmenityRepository.GetByIdAsync(amenityId);
            if (amenity == null)
                throw new KeyNotFoundException("Oda olanağı bulunamadı.");

            selectedAmenities.Add(amenity);
        }

        room.RoomAmenities.Clear();

        foreach (var amenity in selectedAmenities)
        {
            room.RoomAmenities.Add(amenity);
        }

        await _unitOfWork.SaveChangesAsync();
        return room.RoomAmenities.Select(MapToDto).ToList();
    }

    public async Task RemoveFromHotelRoomAsync(int hotelId, int roomId, int roomAmenityId)
    {
        var room = await GetOwnedHotelRoomAsync(hotelId, roomId);
        var amenity = room.RoomAmenities.FirstOrDefault(x => x.Id == roomAmenityId);

        if (amenity == null)
            throw new KeyNotFoundException("Oda olanağı bu odaya ait değil.");

        room.RoomAmenities.Remove(amenity);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<HotelRoom> GetOwnedHotelRoomAsync(int hotelId, int roomId)
    {
        var room = await _unitOfWork.HotelRoomRepository.GetRoomWithDetailsAsync(roomId);

        if (room == null)
            throw new KeyNotFoundException("Oda bulunamadı.");

        if (room.HotelId != hotelId)
            throw new InvalidOperationException("Oda bu otele ait değil.");

        return room;
    }

    private static RoomAmenityDto MapToDto(RoomAmenity amenity)
    {
        return new RoomAmenityDto
        {
            Id = amenity.Id,
            Name = amenity.Name,
            Icon = amenity.Icon
        };
    }

    private static string NormalizeName(string name)
    {
        var normalized = name.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Oda olanağı adı boş olamaz.");

        return normalized;
    }

    private static string? NormalizeIcon(string? icon)
    {
        return string.IsNullOrWhiteSpace(icon) ? null : icon.Trim();
    }
}
