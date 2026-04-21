using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoomTypeService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


        public async Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync()
        {
            var roomTypes = await _unitOfWork.RoomTypeRepository.GetAllAsync();

            return roomTypes.Select(r => new RoomTypeDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                MaxGuests = r.MaxGuests
            });

        }

        public async Task<RoomTypeDto?> GetRoomTypeByIdAsync(int id)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(id);

            if (roomType == null) return null;

            return new RoomTypeDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                MaxGuests = roomType.MaxGuests,
            };
        }

        public async Task<IEnumerable<RoomTypeDto>> GetRoomTypesByMaxGuestsAsync(int maxGuests)
        {
            var roomTypes = await _unitOfWork.RoomTypeRepository.GetByMaxGuestsAsync(maxGuests);

            return roomTypes.Where(x => x.MaxGuests <= maxGuests)
                .Select(x => new RoomTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    MaxGuests = x.MaxGuests,
                });
        }

        public async Task<RoomTypeDto?> GetRoomTypeByNameAsync(string name)
        {
            var entity = await _unitOfWork.RoomTypeRepository.GetByNameAsync(name);

            if (entity == null) return null;

            return new RoomTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                MaxGuests = entity.MaxGuests,
            };
        }

        public async Task AddRoomTypeAsync(CreateRoomTypeDto dto)
        {
            var entity = new RoomType
            {
                Name = dto.Name,
                Description = dto.Description,
                MaxGuests = dto.MaxGuests
            };

            await _unitOfWork.RoomTypeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateRoomTypeAsync(UpdateRoomTypeDto dto)
        {
            var entity = await _unitOfWork.RoomTypeRepository.GetByIdAsync(dto.Id);

            if(entity ==null) throw new KeyNotFoundException("Oda tipi bulunamadı.");

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.MaxGuests = dto.MaxGuests;

            _unitOfWork.RoomTypeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRoomTypeAsync(int id)
        {
            var entity = await _unitOfWork.RoomTypeRepository.GetRoomTypeWithRoomsAsync(id);
            if (entity == null) 
                throw new KeyNotFoundException("Oda tipi bulunamadı.");

            if(entity.HotelRooms.Any())
                throw new InvalidOperationException("Kullanımda olan oda tipi silinemez.");

            _unitOfWork.RoomTypeRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

     

    }
}
