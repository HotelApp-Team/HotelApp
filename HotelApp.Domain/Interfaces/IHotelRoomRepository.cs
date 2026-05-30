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
    /// HotelRoom entity'si icin temel repository islemlerine ek olarak,
    /// otel odalarina ozel sorgulama metotlarini tanimlar.
    /// </summary>
    public interface IHotelRoomRepository : IRepository<HotelRoom>
    {
        /// <summary>
        /// Verilen Id degerine sahip odayi iliskili detay verileriyle birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek otel odasinin Id degeri.</param>
        /// <returns>Oda detaylari; bulunamazsa null.</returns>
        Task<HotelRoom?> GetRoomWithDetailsAsync(int id);

        /// <summary>
        /// Tum odalari iliskili detay verileriyle birlikte getirir.
        /// </summary>
        /// <returns>Detaylariyla birlikte otel odasi listesini doner.</returns>
        Task<IEnumerable<HotelRoom>> GetRoomsWithDetailsAsync();

        /// <summary>
        /// Belirli bir otele ait odalari getirir.
        /// </summary>
        /// <param name="hotelId">Odalari listelenecek otelin Id degeri.</param>
        /// <returns>Belirtilen otele ait oda listesini doner.</returns>
        Task<IEnumerable<HotelRoom>> GetRoomsByHotelAsync(int hotelId);


        /// <summary>
        /// Belirli bir oda tipine sahip odalari getirir.
        /// </summary>
        /// <param name="roomTypeId">Filtreleme icin kullanilacak oda tipi Id degeri.</param>
        /// <returns>Belirtilen oda tipine ait oda listesini doner.</returns>
        Task<IEnumerable<HotelRoom>> GetRoomsByRoomTypeAsync(int roomTypeId);


        /// <summary>
        /// Belirtilen otel, sehir, tarih araligi ve misafir sayisina gore musait odalari getirir.
        /// </summary>
        /// <param name="hotelId">Arama yapilacak otelin Id degeri.</param>
        /// <param name="city">Otelin bulundugu sehir.</param>
        /// <param name="checkIn">Giris tarihi.</param>
        /// <param name="checkOut">Cikis tarihi.</param>
        /// <param name="guestCount">Misafir sayisi.</param>
        /// <returns>Arama kriterlerine uygun musait oda listesini doner.</returns>
        Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(int hotelId, string city, DateTime checkIn, DateTime checkOut, int guestCount);

    }
}
