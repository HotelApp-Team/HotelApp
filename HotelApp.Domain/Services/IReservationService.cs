using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IReservationService
    {
        /// <summary>
        /// Belirtilen kullaniciya ait rezervasyonlari getirir.
        /// </summary>
        /// <param name="userId">Rezervasyonlari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullanicinin rezervasyon DTO listesini doner.</returns>
        Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId);

        /// <summary>
        /// Belirtilen kullaniciya ait rezervasyon detaylarini getirir.
        /// </summary>
        /// <param name="userId">Rezervasyon detaylari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullanicinin rezervasyon detay DTO listesini doner.</returns>
        Task<IEnumerable<ReservationDetailDto>> GetUserReservationDetailsAsync(int userId);

        /// <summary>
        /// Verilen Id degerine sahip rezervasyonu getirir.
        /// </summary>
        /// <param name="id">Getirilecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyon DTO; bulunamazsa null.</returns>
        Task<ReservationDto?> GetReservationByIdAsync(int id);

        /// <summary>
        /// Belirtilen kullanici icin yeni rezervasyon olusturur.
        /// </summary>
        /// <param name="userId">Rezervasyonu olusturan kullanicinin Id degeri.</param>
        /// <param name="dto">Rezervasyon olusturma bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan rezervasyon DTO nesnesini doner.</returns>
        Task<ReservationDto> CreateReservationAsync(int userId, CreateReservationDto dto);

        /// <summary>
        /// Belirtilen rezervasyonu iptal eder.
        /// Admin kullanicilar farkli kullanicilara ait rezervasyonlari da iptal edebilir.
        /// </summary>
        /// <param name="id">Iptal edilecek rezervasyonun Id degeri.</param>
        /// <param name="userId">Islemi yapan kullanicinin Id degeri.</param>
        /// <param name="isAdmin">Islemi yapan kullanicinin admin olup olmadigini belirtir.</param>
        Task CancelReservationAsync(int id, int userId, bool isAdmin);

        /// <summary>
        /// Check-out tarihi gecmis Confirmed rezervasyonlari Completed durumuna gunceller.
        /// Background Service tarafindan periyodik olarak cagrilir.
        /// </summary>
        /// <returns>Guncellenen rezervasyon kaydi sayisini doner.</returns>
        Task<int> MarkPastReservationsCompletedAsync();
    }
}
