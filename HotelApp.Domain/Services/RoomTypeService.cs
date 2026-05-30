using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    /// <summary>
    /// Oda tipleriyle ilgili is kurallarini yoneten servis sinifidir.
    /// Oda tipi listeleme, detay getirme, ekleme, guncelleme ve silme islemlerini gerceklestirir.
    /// </summary>
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// RoomTypeService sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="unitOfWork">Repository islemlerine erisim icin kullanilan UnitOfWork nesnesi.</param>
        public RoomTypeService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        /// <summary>
        /// Tum oda tiplerini getirir.
        /// </summary>
        /// <returns>Oda tipi DTO listesini doner.</returns>
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

        /// <summary>
        /// Verilen Id degerine sahip oda tipini getirir.
        /// </summary>
        /// <param name="id">Getirilecek oda tipinin Id degeri.</param>
        /// <returns>Oda tipi DTO nesnesi; bulunamazsa null.</returns>
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

        /// <summary>
        /// Belirtilen maksimum misafir sayisina uygun oda tiplerini getirir.
        /// </summary>
        /// <param name="maxGuests">Filtreleme icin kullanilacak maksimum misafir sayisi.</param>
        /// <returns>Maksimum misafir sayisina uygun oda tipi DTO listesini doner.</returns>
        public async Task<IEnumerable<RoomTypeDto>> GetRoomTypesByMaxGuestsAsync(int maxGuests)
        {
            var roomTypes = await _unitOfWork.RoomTypeRepository.GetByMaxGuestsAsync(maxGuests);

            return roomTypes.Select(x => new RoomTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                MaxGuests = x.MaxGuests,
            });
        }

        /// <summary>
        /// Belirtilen ada sahip oda tipini getirir.
        /// </summary>
        /// <param name="name">Aranacak oda tipi adi.</param>
        /// <returns>Oda tipi DTO nesnesi; bulunamazsa null.</returns>
        public async Task<RoomTypeDto?> GetRoomTypeByNameAsync(string name)
        {
            var entity = await _unitOfWork.RoomTypeRepository.GetByNameAsync(name);

            if (entity == null) return null;

            return new RoomTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                MaxGuests = entity.MaxGuests,
            };
        }

        /// <summary>
        /// Yeni oda tipi kaydi olusturur.
        /// Ayni isimde oda tipi varsa islem yapilmaz.
        /// </summary>
        /// <param name="dto">Olusturulacak oda tipi bilgilerini iceren DTO.</param>
        /// <exception cref="InvalidOperationException">Ayni isimde oda tipi varsa firlatilir.</exception>
        public async Task AddRoomTypeAsync(CreateRoomTypeDto dto)
        {
            var existing = await _unitOfWork.RoomTypeRepository.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new InvalidOperationException("Bu isimde bir oda tipi zaten mevcut.");

            var entity = new RoomType
            {
                Name = dto.Name,
                Description = dto.Description,
                MaxGuests = dto.MaxGuests
            };

            await _unitOfWork.RoomTypeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Mevcut oda tipi kaydini gunceller.
        /// </summary>
        /// <param name="dto">Guncellenecek oda tipi bilgilerini iceren DTO.</param>
        /// <exception cref="KeyNotFoundException">Oda tipi bulunamazsa firlatilir.</exception>
        public async Task UpdateRoomTypeAsync(UpdateRoomTypeDto dto)
        {
            var entity = await _unitOfWork.RoomTypeRepository.GetByIdAsync(dto.Id);

            if (entity == null) throw new KeyNotFoundException("Oda tipi bulunamadı.");

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.MaxGuests = dto.MaxGuests;

            _unitOfWork.RoomTypeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Belirtilen Id degerine sahip oda tipini siler.
        /// Herhangi bir otel odasinda kullanilan oda tipi silinemez.
        /// </summary>
        /// <param name="id">Silinecek oda tipinin Id degeri.</param>
        /// <exception cref="KeyNotFoundException">Oda tipi bulunamazsa firlatilir.</exception>
        /// <exception cref="InvalidOperationException">Oda tipi kullanimdaysa firlatilir.</exception>
        public async Task DeleteRoomTypeAsync(int id)
        {
            var entity = await _unitOfWork.RoomTypeRepository.GetRoomTypeWithRoomsAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Oda tipi bulunamadı.");

            if (entity.HotelRooms.Any())
                throw new InvalidOperationException("Kullanımda olan oda tipi silinemez.");

            _unitOfWork.RoomTypeRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
