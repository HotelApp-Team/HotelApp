using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    /// <summary>
    /// Otel odalariyla ilgili is kurallarini yoneten servis sinifidir.
    /// Oda ekleme, guncelleme, silme, listeleme ve musait oda arama islemlerini gerceklestirir.
    /// </summary>
    public class HotelRoomService : IHotelRoomService
    {
        /// <summary>
        /// HotelRoomService sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="unitOfWork">Repository islemlerine erisim icin kullanilan UnitOfWork nesnesi.</param>
        private readonly IUnitOfWork _unitOfWork;
        public HotelRoomService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        /// <summary>
        /// Belirtilen otel ve oda tipi icin yeni oda kaydi olusturur.
        /// </summary>
        /// <param name="dto">Olusturulacak oda bilgilerini iceren DTO.</param>
        /// <exception cref="KeyNotFoundException">Hotel veya oda tipi bulunamazsa firlatilir.</exception>
        /// <exception cref="InvalidOperationException">Ayni otel ve oda tipi icin kayit varsa firlatilir.</exception>
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


        /// <summary>
        /// Belirtilen Id degerine sahip oda kaydini siler.
        /// Rezervasyonu olan odalar silinemez.
        /// </summary>
        /// <param name="id">Silinecek oda kaydinin Id degeri.</param>
        /// <exception cref="KeyNotFoundException">Oda bulunamazsa firlatilir.</exception>
        /// <exception cref="InvalidOperationException">Odaya ait rezervasyon varsa firlatilir.</exception>
        public async Task DeleteRoomsAsync(int id)
        {
            var rooms = await _unitOfWork.HotelRoomRepository.GetRoomWithDetailsAsync(id);
            if (rooms == null) throw new KeyNotFoundException("Oda bulunamadı.");

            if (rooms.Reservations.Any())
                throw new InvalidOperationException("Rezervasyonu olan oda silinemez.");

            _unitOfWork.HotelRoomRepository.Delete(rooms);
            await _unitOfWork.SaveChangesAsync();
        }


        /// <summary>
        /// Tum otel odalarini detay bilgileriyle birlikte getirir.
        /// </summary>
        /// <returns>Otel odasi DTO listesini doner.</returns>
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

        /// <summary>
        /// Belirtilen otel, sehir, tarih araligi ve misafir sayisina gore musait odalari getirir.
        /// </summary>
        /// <param name="hotelId">Arama yapilacak otelin Id degeri.</param>
        /// <param name="city">Otelin bulundugu sehir.</param>
        /// <param name="checkIn">Giris tarihi.</param>
        /// <param name="checkOut">Cikis tarihi.</param>
        /// <param name="guestCount">Misafir sayisi.</param>
        /// <returns>Musait oda DTO listesini doner.</returns>
        public async Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(int hotelId, string city, DateTime checkIn, DateTime checkOut, int guestCount)
        {
            return await _unitOfWork.HotelRoomRepository.GetAvailableRoomsAsync(hotelId, city, checkIn, checkOut, guestCount);
        }


        /// <summary>
        /// Belirtilen Id degerine sahip otel odasini detay bilgileriyle birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek oda kaydinin Id degeri.</param>
        /// <returns>Oda detayi; bulunamazsa null.</returns>
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

        /// <summary>
        /// Belirtilen otele ait odalari, oda tipi, imkan ve gorsel bilgileriyle birlikte getirir.
        /// </summary>
        /// <param name="hotelId">Odalari listelenecek otelin Id degeri.</param>
        /// <returns>Belirtilen otele ait oda DTO listesini doner.</returns>
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
                RoomAmenities = MapRoomAmenities(x.RoomAmenities),

                MainImageUrl = x.RoomImages
                .OrderByDescending(i => i.IsMain)
                .ThenBy(i => i.DisplayOrder)
                .Select(i => i.ImageUrl)
                .FirstOrDefault(),

                Images = x.RoomImages
                .OrderByDescending(i => i.IsMain)
                .ThenBy(i => i.DisplayOrder)
                .Select(x => new RoomImageDto
                {
                    Id = x.Id,
                    HotelRoomId = x.HotelRoomId,
                    ImageUrl = x.ImageUrl,
                    IsMain = x.IsMain,
                    DisplayOrder = x.DisplayOrder
                }).ToList()
            });
        }

        /// <summary>
        /// Belirtilen oda tipine ait otel odalarini getirir.
        /// </summary>
        /// <param name="roomTypeId">Odalari listelenecek oda tipinin Id degeri.</param>
        /// <returns>Belirtilen oda tipine ait oda DTO listesini doner.</returns>
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

        /// <summary>
        /// Mevcut otel odasi kaydini gunceller.
        /// Rezervasyonu olan odalarda oda sayisi degistirilemez;
        /// devam eden veya gelecekteki rezervasyonu olan oda pasife cekilemez.
        /// </summary>
        /// <param name="dto">Guncellenecek oda bilgilerini iceren DTO.</param>
        /// <exception cref="KeyNotFoundException">Oda, hotel veya oda tipi bulunamazsa firlatilir.</exception>
        /// <exception cref="InvalidOperationException">Is kurali ihlali durumunda firlatilir.</exception>
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

            // Pasife cekmeyi sadece devam eden veya gelecekteki rezervasyonlar engeller.
            // Gecmiste kalmis (Completed/Cancelled veya cikisi gecmis) rezervasyonlar engel degildir;
            // yoksa bir kez rezervasyon almis oda hicbir zaman satistan kaldirilamazdi.
            var today = DateOnly.FromDateTime(DateTime.Today);
            bool hasOngoingOrFutureReservation = entity.Reservations.Any(r =>
                string.Equals(r.Status, "Confirmed", StringComparison.OrdinalIgnoreCase)
                 && r.CheckOutDate >= today);

            if (hasOngoingOrFutureReservation && !dto.IsActive)
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

        /// <summary>
        /// RoomAmenity entity listesini RoomAmenityDto listesine donusturur.
        /// </summary>
        /// <param name="amenities">Donusturulecek oda imkani listesi.</param>
        /// <returns>RoomAmenityDto listesini doner.</returns>
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
