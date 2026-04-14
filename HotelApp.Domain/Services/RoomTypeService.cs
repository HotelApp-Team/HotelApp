using HotelApp.Domain.DTOs;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository _repository;
        public RoomTypeService(IRoomTypeRepository repository) => _repository = repository;

        public Task AddRoomTypeAsync(RoomTypeDto dto)
        {
            var roomType = new Entities.RoomType
            {
                Name = dto.Name,
                Description = dto.Description,
                MaxGuests = dto.MaxGuests
            };
            return _repository.AddRoomTypeAsync(roomType);
        }

        public Task DeleteRoomTypeAsync(int id)
        {
            var roomType = new Entities.RoomType { Id = id };
            return _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync()
        {
            var roomTypes = await _repository.GetAllAsync();
            return roomTypes.Select(r => new RoomTypeDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                MaxGuests = r.MaxGuests,
                HotelRoomCount = r.HotelRooms.Count().ToString()
            });

        }

        public async Task<RoomTypeDto?> GetRoomTypeByIdAsync(int id)
        {
            var roomType = await _repository.GetByIdAsync(id);
            if (roomType == null) return null;
            return new RoomTypeDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            };
        }

        public async Task<IEnumerable<RoomTypeDto>> GetRoomTypeByMaxGuestsAsync(int maxGuests)
        {
            var roomType = await _repository.GetByMaxGuestsAsync(maxGuests);
            return roomType.Select(r => new RoomTypeDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });
        }

        public async Task<RoomTypeDto?> GetRoomTypeByNameAsync(string name)
        {
            var roomType=await _repository.GetByNameAsync(name);
            if (roomType == null) return null;
            return new RoomTypeDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            };
        }

        public Task UpdateRoomTypeAsync(RoomTypeDto dto)
        {
            var roomType = new Entities.RoomType
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                MaxGuests = dto.MaxGuests
            };
            return _repository.UpdateAsync(roomType);
        } 
    }
}
