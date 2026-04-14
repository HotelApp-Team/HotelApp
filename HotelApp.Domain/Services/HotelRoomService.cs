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
        private readonly IHotelRoomRepository _repository;
        public HotelRoomService(IHotelRoomRepository repository) => _repository = repository;

        public Task AddRoomsAsync(CreateHotelRoomDto dto)
        {
            var entity = new Entities.HotelRoom
            {
                HotelId = dto.HotelId,
                RoomTypeId = dto.RoomTypeId,
                PricePerNight = dto.PricePerNight,
                TotalRooms = dto.TotalRooms,
                IsActive = true
            };
            return _repository.AddAsync(entity);
        }

        public async Task DeleteRoomsAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<HotelRoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _repository.GetRoomsWithDetailsAsync();
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
            var checkInDateOnly = DateOnly.FromDateTime(checkIn);
            var checkOutDateOnly = DateOnly.FromDateTime(checkOut);

            var rooms = await _repository.GetAvailableRoomsAsync(checkIn, checkOut);

            return rooms.Select(x => new AvailableRoomDto
            {
                RoomId = x.Id,
                HotelName = x.Hotel.Name,
                RoomTypeName = x.RoomType.Name,
                PricePerNight = x.PricePerNight,
                Available = x.TotalRooms - x.Reservations.Count(r =>
                r.CheckInDate < checkOutDateOnly && r.CheckOutDate > checkInDateOnly)
            });
        }

        public async Task<DetailHotelRoomDto?> GetRoomByIdAsync(int id)
        {
            var room = await _repository.GetRoomWithDetailsAsync(id);
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
            var rooms = await _repository.GetRoomsWithDetailsAsync();
            return rooms
               .Where(x => x.HotelId == x.HotelId)
               .Select(x => new HotelRoomDto
               {
                   Id = x.Id,
                   PricePerNight = x.PricePerNight,
                   TotalRooms = x.TotalRooms,
                   HotelName = x.Hotel.Name,
                   RoomTypeName = x.RoomType.Name
               });
        }

        public async Task<IEnumerable<HotelRoomDto>> GetRoomsByTypeAsync(int roomTypeId)
        {
            var rooms = await _repository.GetRoomsByRoomTypeAsync(roomTypeId);
            return rooms.Select(x => new HotelRoomDto
            {
                Id = x.Id,
                PricePerNight = x.PricePerNight,
                TotalRooms = x.TotalRooms,
                HotelName = x.Hotel.Name,
                RoomTypeName = x.RoomType.Name
            });
        }

        public async Task UpdateRoomsAsync(UpdateHotelRoomDto dto)
        {
            var entity = await _repository.GetRoomWithDetailsAsync(dto.RoomTypeId);
            if (entity == null) return;
            entity.HotelId = dto.HotelId;
            entity.RoomTypeId = dto.RoomTypeId;
            entity.PricePerNight = dto.PricePerNight;
            entity.TotalRooms = dto.TotalRooms;
            entity.IsActive = dto.IsActive;

            await _repository.UpdateAsync(entity);
        }
    }
}
