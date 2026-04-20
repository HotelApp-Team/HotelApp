using HotelApp.Domain.DTOs;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public class HotelRoomService : IHotelRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        public HotelRoomService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task AddRoomsAsync(CreateHotelRoomDto dto)
        {
            var entity = new Entities.HotelRoom
            {
                HotelId = dto.HotelId,
                RoomTypeId = dto.RoomTypeId,
                PricePerNight = dto.PricePerNight,
                TotalRooms = dto.TotalRooms,
                IsActive = true
            };

            await _unitOfWork.HotelRoomRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRoomsAsync(int id)
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetByIdAsync(id);
            if (rooms == null) throw new Exception("Oda bulunamadı.");

            _unitOfWork.HotelRoomRepository.Delete(rooms);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<HotelRoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetRoomsWithDetailsAsync();

            return rooms.Select(x => new HotelRoomDto
            {
                Id = x.Id,
                PricePerNight = x.PricePerNight,
                TotalRooms = x.TotalRooms,
                HotelName = x.Hotel.Name,
                RoomTypeName = x.RoomType.Name
            });
        }

        public async Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            return await _unitOfWork.HotelRoomRepository.GetAvailableRoomsAsync(checkIn, checkOut);
        }

        public async Task<DetailHotelRoomDto?> GetRoomByIdAsync(int id)
        {
            var room = await _unitOfWork.HotelRoomRepository.GetRoomWithDetailsAsync(id);

            if (room == null) return null;

            return new DetailHotelRoomDto
            {
                Id = room.Id,
                HotelId = room.HotelId,
                RoomTypeId = room.RoomTypeId,
                PricePerNight = room.PricePerNight,
                TotalRooms = room.TotalRooms,
                IsActive = room.IsActive,
                HotelName = room.Hotel.Name,
                RoomTypeName = room.RoomType.Name,
                ReservationCount = room.Reservations.Count
            };
        }

        public async Task<IEnumerable<HotelRoomDto>> GetRoomsByHotelAsync(int hotelId)
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetRoomsByHotelAsync(hotelId);

            return rooms.Select(x => new HotelRoomDto
            {
                Id = x.Id,
                PricePerNight = x.PricePerNight,
                TotalRooms = x.TotalRooms,
                HotelName = x.Hotel?.Name,
                RoomTypeName = x.RoomType?.Name
            });
        }

        public async Task<IEnumerable<HotelRoomDto>> GetRoomsByTypeAsync(int roomTypeId)
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetRoomsByRoomTypeAsync(roomTypeId);
            return rooms.Select(x => new HotelRoomDto
            {
                Id = x.Id,
                PricePerNight = x.PricePerNight,
                TotalRooms = x.TotalRooms,
                HotelName = x.Hotel?.Name,
                RoomTypeName = x.RoomType?.Name
            });
        }

        public async Task UpdateRoomsAsync(UpdateHotelRoomDto dto)
        {
            var entity = await _unitOfWork.HotelRoomRepository.GetRoomWithDetailsAsync(dto.Id);

            if (entity == null)
                throw new Exception("Oda bulunamadı");

            entity.HotelId = dto.HotelId;
            entity.RoomTypeId = dto.RoomTypeId;
            entity.PricePerNight = dto.PricePerNight;
            entity.TotalRooms = dto.TotalRooms;
            entity.IsActive = dto.IsActive;

            _unitOfWork.HotelRoomRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
