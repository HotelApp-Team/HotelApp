using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IRoomImageService
    {
        /// <summary>
        /// Belirtilen otel odasina ait gorselleri getirir.
        /// </summary>
        /// <param name="hotelRoomId">Gorselleri listelenecek otel odasinin Id degeri.</param>
        /// <returns>Odaya ait gorsel DTO listesini doner.</returns>
        Task<List<RoomImageDto>> GetByRoomIdAsync(int hotelRoomId);

        /// <summary>
        /// Yeni oda gorseli kaydi olusturur.
        /// </summary>
        /// <param name="dto">Olusturulacak oda gorseli bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan oda gorseli DTO nesnesini doner; olusturulamazsa null.</returns>
        Task<RoomImageDto?> AddAsync(CreateRoomImageDto dto);

        /// <summary>
        /// Belirtilen Id degerine sahip oda gorselini siler.
        /// </summary>
        /// <param name="id">Silinecek oda gorselinin Id degeri.</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Belirtilen gorseli ilgili otel odasi icin ana gorsel olarak isaretler.
        /// </summary>
        /// <param name="hotelRoomId">Ana gorseli guncellenecek otel odasinin Id degeri.</param>
        /// <param name="imageId">Ana gorsel olarak isaretlenecek gorselin Id degeri.</param>
        Task SetMainAsync(int hotelRoomId, int imageId);

        /// <summary>
        /// Belirtilen oda gorselini gunceller.
        /// </summary>
        /// <param name="roomId">Gorselin ait oldugu otel odasinin Id degeri.</param>
        /// <param name="imageId">Guncellenecek gorselin Id degeri.</param>
        /// <param name="dto">Guncel oda gorseli bilgilerini iceren DTO.</param>
        Task UpdateAsync(int roomId, int imageId, UpdateRoomImageDto dto);
    }
}
