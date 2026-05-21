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

            // Manzara da unique key'in parcasi: ayni otel + ayni oda tipi + ayni
            // manzara kombinasyonunda tekrar kayit acilamaz. Suit-Deniz varken
            // Suit-Sehir eklenebilir cunku manzaralar farkli.
            var existing = await _unitOfWork.HotelRoomRepository
                .FindAsync(x => x.HotelId == dto.HotelId
                             && x.RoomTypeId == dto.RoomTypeId
                             && x.RoomViewId == dto.RoomViewId);

            if (existing != null) throw new InvalidOperationException("Aynı otel, oda tipi ve manzara için zaten kayıt mevcut.");

            var entity = new HotelRoom
            {
                HotelId = dto.HotelId,
                RoomTypeId = dto.RoomTypeId,
                PricePerNight = dto.PricePerNight,
                ExtraGuestPricePerNight = dto.ExtraGuestPricePerNight,
                TotalRooms = dto.TotalRooms,
                RoomViewId = dto.RoomViewId,
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
                IsActive = x.IsActive,
                HotelName = x.Hotel?.Name ?? string.Empty,
                RoomTypeName = x.RoomType?.Name ?? string.Empty,
                RoomViewId = x.RoomViewId,
                RoomViewName = x.RoomView?.Name,
                RoomViewIcon = x.RoomView?.Icon,
                RoomAmenities = MapRoomAmenities(x.RoomAmenities)
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
                RoomViewId = room.RoomViewId,
                RoomViewName = room.RoomView?.Name,
                RoomViewIcon = room.RoomView?.Icon,
                ReservationCount = room.Reservations.Count,
                RoomAmenities = MapRoomAmenities(room.RoomAmenities)
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
                IsActive = x.IsActive,
                HotelName = x.Hotel?.Name ?? string.Empty,
                RoomTypeName = x.RoomType?.Name ?? string.Empty,
                MaxGuests = x.RoomType?.MaxGuests ?? 0,
                RoomViewId = x.RoomViewId,
                RoomViewName = x.RoomView?.Name,
                RoomViewIcon = x.RoomView?.Icon,
                RoomAmenities = MapRoomAmenities(x.RoomAmenities)
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
                IsActive = x.IsActive,
                HotelName = x.Hotel?.Name ?? string.Empty,
                RoomTypeName = x.RoomType?.Name ?? string.Empty,
                RoomViewId = x.RoomViewId,
                RoomViewName = x.RoomView?.Name,
                RoomViewIcon = x.RoomView?.Icon,
                RoomAmenities = MapRoomAmenities(x.RoomAmenities)
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

            // Duplicate kontrolu manzara dahil uclu kombinasyonla yapilir:
            // ayni otelde ayni oda tipi + ayni manzara baska bir kayitta varsa engelle.
            var duplicate = await _unitOfWork.HotelRoomRepository.FindAsync(x =>
                x.Id != dto.Id &&
                x.HotelId == dto.HotelId &&
                x.RoomTypeId == dto.RoomTypeId &&
                x.RoomViewId == dto.RoomViewId);

            if (duplicate != null)
                throw new InvalidOperationException("Bu otelde bu oda tipi ve manzara kombinasyonu zaten mevcut.");

            if (entity.Reservations.Any() && entity.TotalRooms != dto.TotalRooms)
                throw new InvalidOperationException("Rezervasyon varken oda sayısı değiştirilemez.");

            if (entity.Reservations.Any() && !dto.IsActive)
                throw new InvalidOperationException("Aktif rezervasyonu olan oda pasife çekilemez.");

            entity.HotelId = dto.HotelId;
            entity.RoomTypeId = dto.RoomTypeId;
            entity.PricePerNight = dto.PricePerNight;
            entity.ExtraGuestPricePerNight = dto.ExtraGuestPricePerNight;
            entity.TotalRooms = dto.TotalRooms;
            entity.RoomViewId = dto.RoomViewId;
            entity.IsActive = dto.IsActive;

            _unitOfWork.HotelRoomRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private static List<RoomAmenityDto> MapRoomAmenities(IEnumerable<RoomAmenity> amenities)
        {
            return amenities.Select(a => new RoomAmenityDto
            {
                Id = a.Id,
                Name = a.Name,
                Icon = a.Icon
            }).ToList();
        }
    }
}
