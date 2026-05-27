using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    /// <summary>
    /// RoomImage entity'si icin temel repository islemlerine ek olarak,
    /// oda gorsellerine ozel sorgulama ve ana gorsel belirleme metotlarini tanimlar.
    /// </summary>
    public interface IRoomImageRepository : IRepository<RoomImage>
    {

        /// <summary>
        /// Belirtilen otel odasina ait tum gorselleri getirir.
        /// </summary>
        /// <param name="hotelRoomId">Gorselleri listelenecek otel odasinin Id degeri.</param>
        /// <returns>Belirtilen odaya ait gorsel listesini doner.</returns>
        Task<List<RoomImage>> GetByHotelRoomIdAsync(int hotelRoomId);

        /// <summary>
        /// Belirtilen otel odasina ait ana gorseli getirir.
        /// </summary>
        /// <param name="hotelRoomId">Ana gorseli aranacak otel odasinin Id degeri.</param>
        /// <returns>Ana gorsel; bulunamazsa null.</returns>
        Task<RoomImage?> GetMainImageAsync(int hotelRoomId);

        /// <summary>
        /// Belirtilen gorseli, ilgili otel odasi icin ana gorsel olarak isaretler.
        /// </summary>
        /// <param name="hotelRoomId">Ana gorseli guncellenecek otel odasinin Id degeri.</param>
        /// <param name="imageId">Ana gorsel olarak isaretlenecek gorselin Id degeri.</param>
        Task SetMainAsync(int hotelRoomId, int imageId);
    }
}
