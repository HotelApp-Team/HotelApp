using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    /// <summary>
    /// Otel odalari icin listeleme, detay getirme, musaitlik arama,
    /// ekleme, guncelleme ve silme islemlerini tanimlar.
    /// </summary>
    public interface IHotelRoomService
    {
        /// <summary>
        /// Tum otel odalarini liste halinde getirir.
        /// </summary>
        /// <returns>Otel odasi DTO listesini doner.</returns>
        Task<IEnumerable<HotelRoomDto>> GetAllRoomsAsync();

        /// <summary>
        /// Belirtilen Id degerine sahip otel odasinin detaylarini getirir.
        /// </summary>
        /// <param name="id">Detaylari getirilecek otel odasinin Id degeri.</param>
        /// <returns>Otel odasi detayi; bulunamazsa null.</returns>
        Task<DetailHotelRoomDto?> GetRoomByIdAsync(int id);

        /// <summary>
        /// Belirtilen otele ait odalari getirir.
        /// </summary>
        /// <param name="hotelId">Odalari listelenecek otelin Id degeri.</param>
        /// <returns>Belirtilen otele ait oda DTO listesini doner.</returns>
        Task<IEnumerable<HotelRoomDto>> GetRoomsByHotelAsync(int hotelId);

        /// <summary>
        /// Belirtilen oda tipine ait odalari getirir.
        /// </summary>
        /// <param name="roomTypeId">Odalari listelenecek oda tipinin Id degeri.</param>
        /// <returns>Belirtilen oda tipine ait oda DTO listesini doner.</returns>
        Task<IEnumerable<HotelRoomDto>> GetRoomsByTypeAsync(int roomTypeId);

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
        
        /// <summary>
        /// Yeni otel odasi kaydi olusturur.
        /// </summary>
        /// <param name="dto">Olusturulacak otel odasi bilgilerini iceren DTO.</param>
        Task AddRoomsAsync(CreateHotelRoomDto dto);

        /// <summary>
        /// Mevcut otel odasi kaydini gunceller.
        /// </summary>
        /// <param name="dto">Guncellenecek otel odasi bilgilerini iceren DTO.</param>
        Task UpdateRoomsAsync(UpdateHotelRoomDto dto);

        /// <summary>
        /// Belirtilen Id degerine sahip otel odasi kaydini siler.
        /// </summary>
        /// <param name="id">Silinecek otel odasinin Id degeri.</param>
        Task DeleteRoomsAsync(int id);
    }
}

