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
    /// Oda gorsellerine ait is kurallarini yoneten servis sinifidir.
    /// Gorsel listeleme, ekleme, silme, guncelleme ve ana gorsel belirleme islemlerini gerceklestirir.
    /// </summary>
    public class RoomImageService : IRoomImageService
    {
        /// <summary>
        /// RoomImageService sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="unitOfWork">Repository islemlerine erisim icin kullanilan UnitOfWork nesnesi.</param>
        private readonly IUnitOfWork _unitOfWork;

        public RoomImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Belirtilen odaya ait tum gorselleri getirir.
        /// </summary>
        /// <param name="hotelRoomId">Gorselleri listelenecek otel odasinin Id degeri.</param>
        /// <returns>Odaya ait gorsel DTO listesini doner.</returns>
        public async Task<List<RoomImageDto>> GetByRoomIdAsync(int hotelRoomId)
        {
            var images = await _unitOfWork.RoomImageRepository.GetByHotelRoomIdAsync(hotelRoomId);

            return images.Select(img => new RoomImageDto
            {
                Id = img.Id,
                HotelRoomId = img.HotelRoomId,
                ImageUrl = img.ImageUrl,
                IsMain = img.IsMain,
                DisplayOrder = img.DisplayOrder
            }).ToList();
        }

        /// <summary>
        /// Yeni oda gorseli kaydi olusturur.
        /// URL bos olamaz, http:// veya https:// ile baslamali ve 500 karakteri gecmemelidir.
        /// </summary>
        /// <param name="dto">Olusturulacak oda gorseli bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan oda gorseli DTO nesnesini doner.</returns>
        /// <exception cref="Exception">Gorsel URL validasyonu basarisiz olursa firlatilir.</exception>
        public async Task<RoomImageDto?> AddAsync(CreateRoomImageDto dto)
        {
            if (string.IsNullOrEmpty(dto.ImageUrl))
                throw new Exception("Görsel URL boş olamaz.");

            if (!dto.ImageUrl.StartsWith("http://") && !dto.ImageUrl.StartsWith("https://"))
                throw new Exception("Görsel URL http:// veya https:// ile başlamalıdır");

            if (dto.ImageUrl.Length > 500)
                throw new Exception("Görsel URL en fazla 500 karakter olabilir.");

            if (dto.IsMain)
                await _unitOfWork.RoomImageRepository.SetMainAsync(dto.HotelRoomId, 0);

            var entity = new RoomImage
            {
                HotelRoomId = dto.HotelRoomId,
                ImageUrl = dto.ImageUrl,
                IsMain = dto.IsMain,
                DisplayOrder = dto.DisplayOrder
            };

            await _unitOfWork.RoomImageRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new RoomImageDto
            {
                Id = entity.Id,
                HotelRoomId = entity.HotelRoomId,
                ImageUrl = entity.ImageUrl,
                IsMain = entity.IsMain,
                DisplayOrder = entity.DisplayOrder
            };
        }

        /// <summary>
        /// Belirtilen Id degerine sahip oda gorselini siler.
        /// </summary>
        /// <param name="id">Silinecek oda gorselinin Id degeri.</param>
        /// <exception cref="Exception">Gorsel bulunamazsa firlatilir.</exception>
        public async Task DeleteAsync(int id)
        {
            var image = await _unitOfWork.RoomImageRepository.GetByIdAsync(id);

            if (image == null)
                throw new Exception("Görsel bulunamadı.");

            _unitOfWork.RoomImageRepository.Delete(image);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Belirtilen gorseli ilgili otel odasi icin ana gorsel olarak isaretler.
        /// </summary>
        /// <param name="hotelRoomId">Ana gorseli guncellenecek otel odasinin Id degeri.</param>
        /// <param name="imageId">Ana gorsel olarak isaretlenecek gorselin Id degeri.</param>
        public async Task SetMainAsync(int hotelRoomId, int imageId)
        {
            await _unitOfWork.RoomImageRepository.SetMainAsync(hotelRoomId, imageId);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Belirtilen oda gorselini gunceller.
        /// Gorselin ilgili odaya ait olup olmadigini ve URL formatini kontrol eder.
        /// </summary>
        /// <param name="roomId">Gorselin ait oldugu otel odasinin Id degeri.</param>
        /// <param name="imageId">Guncellenecek gorselin Id degeri.</param>
        /// <param name="dto">Guncel oda gorseli bilgilerini iceren DTO.</param>
        /// <exception cref="Exception">Gorsel bulunamazsa, oda ile eslesmezse veya URL gecersizse firlatilir.</exception>
        public async Task UpdateAsync(int roomId, int imageId, UpdateRoomImageDto dto)
        {
            var image = await _unitOfWork.RoomImageRepository.GetByIdAsync(imageId);

            if (image == null)
                throw new Exception("Görsel bulunamadı.");

            if (image.HotelRoomId != roomId)
                throw new Exception("Görsel bu odaya ait değil.");

            if (string.IsNullOrWhiteSpace(dto.ImageUrl))
                throw new Exception("Görsel URL boş olamaz.");

            if (!dto.ImageUrl.StartsWith("http://") && !dto.ImageUrl.StartsWith("https://"))
                throw new Exception("Görsel URL http:// veya https:// ile başlamalıdır.");

            image.ImageUrl = dto.ImageUrl;
            image.IsMain = dto.IsMain;
            image.DisplayOrder = dto.DisplayOrder;

            if (dto.IsMain)
                await _unitOfWork.RoomImageRepository.SetMainAsync(roomId, imageId);

            _unitOfWork.RoomImageRepository.Update(image);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
