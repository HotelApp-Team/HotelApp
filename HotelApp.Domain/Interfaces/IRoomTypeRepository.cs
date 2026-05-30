using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{

    /// <summary>
    /// RoomType entity'si icin temel repository islemlerine ek olarak,
    /// oda tiplerine ozel sorgulama metotlarini tanimlar.
    /// </summary>
    public interface IRoomTypeRepository : IRepository<RoomType>
    {

        /// <summary>
        /// Verilen Id degerine sahip oda tipini, iliskili odalariyla birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek oda tipinin Id degeri.</param>
        /// <returns>Oda tipi ve iliskili odalari; bulunamazsa null.</returns>
        Task<RoomType?> GetRoomTypeWithRoomsAsync(int id);

        /// <summary>
        /// Tum oda tiplerini, iliskili odalariyla birlikte getirir.
        /// </summary>
        /// <returns>Odalariyla birlikte oda tipi listesini doner.</returns>
        Task<IEnumerable<RoomType>> GetAllWithRoomsAsync();

        /// <summary>
        /// Belirtilen maksimum misafir kapasitesine uygun oda tiplerini getirir.
        /// </summary>
        /// <param name="maxGuests">Filtreleme icin kullanilacak maksimum misafir sayisi.</param>
        /// <returns>Maksimum misafir kapasitesine uygun oda tipi listesini doner.</returns>
        Task<IEnumerable<RoomType>> GetByMaxGuestsAsync(int maxGuests);

        /// <summary>
        /// Verilen ada sahip oda tipini getirir.
        /// </summary>
        /// <param name="name">Aranacak oda tipi adi.</param>
        /// <returns>Oda tipi; bulunamazsa null.</returns>
        Task<RoomType?> GetByNameAsync(string name);

    }
}
