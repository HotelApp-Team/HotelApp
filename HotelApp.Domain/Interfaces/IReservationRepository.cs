using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        /// <summary>
        /// Belirtilen kullaniciya ait rezervasyonlari getirir.
        /// </summary>
        /// <param name="userId">Rezervasyonlari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullanicinin rezervasyon listesini doner.</returns>
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Verilen Id degerine sahip rezervasyonu detay bilgileriyle birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyon detayi; bulunamazsa null.</returns>
        Task<Reservation?> GetWithDetailsAsync(int id);

        /// <summary>
        /// Belirtilen oda, kullanici, tarih araligi, misafir sayisi ve odeme yontemine gore yeni rezervasyon olusturur.
        /// </summary>
        /// <param name="hotelRoomId">Rezervasyon yapilacak otel odasinin Id degeri.</param>
        /// <param name="userId">Rezervasyonu yapan kullanicinin Id degeri.</param>
        /// <param name="checkInDate">Giris tarihi.</param>
        /// <param name="checkOutDate">Cikis tarihi.</param>
        /// <param name="guestCount">Misafir sayisi.</param>
        /// <param name="paymentMethod">Odeme yontemi.</param>
        /// <returns>Olusturulan rezervasyon kaydini doner.</returns>
        Task<Reservation> MakeReservationAsync(
            int hotelRoomId,
            int userId,
            DateOnly checkInDate,
            DateOnly checkOutDate,
            int guestCount,
            string paymentMethod);

        /// <summary>
        /// Belirtilen kullaniciya ait rezervasyon detaylarini view uzerinden getirir.
        /// </summary>
        /// <param name="userId">Rezervasyon detaylari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullanicinin rezervasyon detay listesini doner.</returns>
        Task<IEnumerable<VwReservationDetail>> GetReservationDetailsByUserIdAsync(int userId);

        // Check-out tarihi gecmis Confirmed rezervasyonlari tek SQL UPDATE'i ile Completed yapar.
        // Donus: etkilenen kayit sayisi. Periyodik calistirma icin Background Service tarafindan cagrilir.
        Task<int> MarkPastConfirmedAsCompletedAsync();

        // Kullanicinin verilen otelde tamamlanmis konaklamasi var mi?
        // Yorum yazma yetkisi icin ReviewService tarafindan cagrilir.
        // Esnek tanim: Status="Completed" VEYA (Status="Confirmed" AND CheckOutDate gecmis).
        // Boylece background service henuz Completed'a cevirmemis olsa bile
        // konaklamasi biten kullanici yorum yazabilir.
        Task<bool> HasCompletedStayAsync(int userId, int hotelId);
    }
}
