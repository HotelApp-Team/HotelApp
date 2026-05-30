using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IRoomTypeService
    {
        /// <summary>
        /// Tum oda tiplerini getirir.
        /// </summary>
        /// <returns>Oda tipi DTO listesini doner.</returns>
        Task<IEnumerable<RoomTypeDto>> GetAllRoomTypesAsync();

        /// <summary>
        /// Belirtilen Id degerine sahip oda tipini getirir.
        /// </summary>
        /// <param name="id">Getirilecek oda tipinin Id degeri.</param>
        /// <returns>Oda tipi DTO nesnesi; bulunamazsa null.</returns>
        Task<RoomTypeDto?> GetRoomTypeByIdAsync(int id);

        /// <summary>
        /// Belirtilen ada sahip oda tipini getirir.
        /// </summary>
        /// <param name="name">Aranacak oda tipi adi.</param>
        /// <returns>Oda tipi DTO nesnesi; bulunamazsa null.</returns>
        Task<RoomTypeDto?> GetRoomTypeByNameAsync(string name);

        /// <summary>
        /// Belirtilen maksimum misafir kapasitesine gore oda tiplerini getirir.
        /// </summary>
        /// <param name="maxGuests">Filtreleme icin kullanilacak maksimum misafir sayisi.</param>
        /// <returns>Kapasite kriterine uygun oda tipi DTO listesini doner.</returns>
        Task<IEnumerable<RoomTypeDto>> GetRoomTypesByMaxGuestsAsync(int maxGuests);

        /// <summary>
        /// Yeni oda tipi kaydi olusturur.
        /// </summary>
        /// <param name="dto">Olusturulacak oda tipi bilgilerini iceren DTO.</param>
        Task AddRoomTypeAsync(CreateRoomTypeDto dto);

        /// <summary>
        /// Mevcut oda tipi kaydini gunceller.
        /// </summary>
        /// <param name="dto">Guncellenecek oda tipi bilgilerini iceren DTO.</param>
        Task UpdateRoomTypeAsync(UpdateRoomTypeDto dto);

        /// <summary>
        /// Belirtilen Id degerine sahip oda tipini siler.
        /// </summary>
        /// <param name="id">Silinecek oda tipinin Id degeri.</param>
        Task DeleteRoomTypeAsync(int id);
    }
}
