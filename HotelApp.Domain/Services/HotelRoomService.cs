using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    public class HotelRoomService : IHotelRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        public HotelRoomService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task AddRoomsAsync(CreateHotelRoomDto dto)
        {
            var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(dto.HotelId);
            if (hotel == null) throw new KeyNotFoundException("Hotel bulunamadı.");

            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null) throw new KeyNotFoundException("Oda tipi bulunamadı.");

            var existing = await _unitOfWork.HotelRoomRepository
                .FindAsync(x => x.HotelId == dto.HotelId && x.RoomTypeId == dto.RoomTypeId);

            if (existing != null) throw new InvalidOperationException("Aynı otel ve oda tipi için zaten kayıt mevcut.");

            var entity = new HotelRoom
            {
                HotelId = dto.HotelId,
                RoomTypeId = dto.RoomTypeId,
                PricePerNight = dto.PricePerNight,
                ExtraGuestPricePerNight = dto.ExtraGuestPricePerNight,
                TotalRooms = dto.TotalRooms,
                IsActive = true
            };

            await _unitOfWork.HotelRoomRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRoomsAsync(int id)
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetRoomWithDetailsAsync(id);
            if (rooms == null) throw new KeyNotFoundException("Oda bulunamadı.");

            if (rooms.Reservations.Any())
                throw new InvalidOperationException("Rezervasyonu olan oda silinemez.");

            _unitOfWork.HotelRoomRepository.Delete(rooms);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<HotelRoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetRoomsWithDetailsAsync();

            return rooms.Select(x => new HotelRoomDto
            {
                Id = x.Id,
                HotelId = x.HotelId,
                RoomTypeId = x.RoomTypeId,
                PricePerNight = x.PricePerNight,
                ExtraGuestPricePerNight = x.ExtraGuestPricePerNight,
                TotalRooms = x.TotalRooms,
                HotelName = x.Hotel?.Name ?? string.Empty,
                RoomTypeName = x.RoomType?.Name ?? string.Empty
            });
        }

        public async Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(int hotelId, string city, DateTime checkIn, DateTime checkOut, int guestCount)
        {
            return await _unitOfWork.HotelRoomRepository.GetAvailableRoomsAsync(hotelId, city, checkIn, checkOut, guestCount);
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
                ExtraGuestPricePerNight = room.ExtraGuestPricePerNight,
                TotalRooms = room.TotalRooms,
                IsActive = room.IsActive,
                HotelName = room.Hotel?.Name ?? string.Empty,
                RoomTypeName = room.RoomType?.Name ?? string.Empty,
                ReservationCount = room.Reservations.Count
            };
        }

        public async Task<IEnumerable<HotelRoomDto>> GetRoomsByHotelAsync(int hotelId)
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetRoomsByHotelAsync(hotelId);

            return rooms.Select(x => new HotelRoomDto
            {
                Id = x.Id,
                HotelId = x.HotelId,
                RoomTypeId = x.RoomTypeId,
                PricePerNight = x.PricePerNight,
                ExtraGuestPricePerNight = x.ExtraGuestPricePerNight,
                TotalRooms = x.TotalRooms,
                HotelName = x.Hotel?.Name ?? string.Empty,
                RoomTypeName = x.RoomType?.Name ?? string.Empty,
                MaxGuests = x.RoomType?.MaxGuests ?? 0
            });
        }

        public async Task<IEnumerable<HotelRoomDto>> GetRoomsByTypeAsync(int roomTypeId)
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetRoomsByRoomTypeAsync(roomTypeId);
            return rooms.Select(x => new HotelRoomDto
            {
                Id = x.Id,
                HotelId = x.HotelId,
                RoomTypeId = x.RoomTypeId,
                PricePerNight = x.PricePerNight,
                ExtraGuestPricePerNight = x.ExtraGuestPricePerNight,
                TotalRooms = x.TotalRooms,
                HotelName = x.Hotel?.Name ?? string.Empty,
                RoomTypeName = x.RoomType?.Name ?? string.Empty
            });
        }

        public async Task UpdateRoomsAsync(UpdateHotelRoomDto dto)
        {
            var entity = await _unitOfWork.HotelRoomRepository.GetRoomWithDetailsAsync(dto.Id);
            if (entity == null)
                throw new KeyNotFoundException("Oda bulunamadı");

            var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(dto.HotelId);
            if (hotel == null)
                throw new KeyNotFoundException("Hotel bulunamadı.");

            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                throw new KeyNotFoundException("Oda tipi bulunamadı.");

            var duplicate = await _unitOfWork.HotelRoomRepository.FindAsync(x =>
                x.Id != dto.Id &&
                x.HotelId == dto.HotelId &&
                x.RoomTypeId == dto.RoomTypeId);

            if (duplicate != null)
                throw new InvalidOperationException("Bu otelde bu oda tipi zaten mevcut.");

            if (entity.Reservations.Any() && entity.TotalRooms != dto.TotalRooms)
                throw new InvalidOperationException("Rezervasyon varken oda sayısı değiştirilemez.");

            entity.HotelId = dto.HotelId;
            entity.RoomTypeId = dto.RoomTypeId;
            entity.PricePerNight = dto.PricePerNight;
            entity.ExtraGuestPricePerNight = dto.ExtraGuestPricePerNight;
            entity.TotalRooms = dto.TotalRooms;
            entity.IsActive = dto.IsActive;

            _unitOfWork.HotelRoomRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
