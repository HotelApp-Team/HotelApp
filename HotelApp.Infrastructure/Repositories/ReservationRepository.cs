using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Infrastructure.Repositories
{
    /// <summary>
    /// Reservation entity'si icin veritabani erisim islemlerini yoneten repository sinifidir.
    /// Kullanici rezervasyonlari, rezervasyon detaylari, rezervasyon olusturma,
    /// gecmis rezervasyonlari tamamlama ve konaklama gecmisi kontrolu sorgularini gerceklestirir.
    /// </summary>
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        /// <summary>
        /// ReservationRepository sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="context">Veritabani islemleri icin kullanilan DbContext nesnesi.</param>
        public ReservationRepository(HotelAppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Belirtilen kullaniciya ait rezervasyonlari otel odasi, otel ve oda tipi detaylariyla birlikte getirir.
        /// </summary>
        /// <param name="userId">Rezervasyonlari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullaniciya ait rezervasyon listesini doner.</returns>
        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.HotelRoom)
                .ThenInclude(x => x.Hotel)
                .Include(x => x.HotelRoom)
                .ThenInclude(x => x.RoomType)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Belirtilen kullaniciya ait rezervasyon detaylarini view uzerinden getirir.
        /// </summary>
        /// <param name="userId">Rezervasyon detaylari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullaniciya ait rezervasyon detay listesini doner.</returns>
        public async Task<IEnumerable<VwReservationDetail>> GetReservationDetailsByUserIdAsync(int userId)
        {
            return await _context.VwReservationDetails
                 .Where(x => x.UserId == userId)
                 .OrderByDescending(x => x.CreatedAt)
                 .ToListAsync();
        }

        /// <summary>
        /// Verilen Id degerine sahip rezervasyonu kullanici, otel odasi, otel ve oda tipi detaylariyla birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyon detayi; bulunamazsa null.</returns>
        public async Task<Reservation?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.HotelRoom)
                .ThenInclude(x => x.Hotel)
                .Include(x => x.HotelRoom)
                .ThenInclude(x => x.RoomType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// sp_MakeReservation stored procedure'unu kullanarak yeni rezervasyon olusturur.
        /// Procedure sonucunda olusan ReservationId degeriyle rezervasyon kaydini tekrar okur.
        /// </summary>
        /// <param name="hotelRoomId">Rezervasyon yapilacak otel odasinin Id degeri.</param>
        /// <param name="userId">Rezervasyonu yapan kullanicinin Id degeri.</param>
        /// <param name="checkInDate">Giris tarihi.</param>
        /// <param name="checkOutDate">Cikis tarihi.</param>
        /// <param name="guestCount">Misafir sayisi.</param>
        /// <param name="paymentMethod">Odeme yontemi.</param>
        /// <returns>Olusturulan rezervasyon kaydini doner.</returns>
        /// <exception cref="InvalidOperationException">Procedure rezervasyon olusturamazsa veya olusan kayit geri okunamazsa firlatilir.</exception>
        public async Task<Reservation> MakeReservationAsync(int hotelRoomId, int userId, DateOnly checkInDate, DateOnly checkOutDate, int guestCount, string paymentMethod)
        {
            var hotelRoomParam = new SqlParameter("@HotelRoomId", hotelRoomId);
            var userIdParam = new SqlParameter("@UserId", userId);
            var checkInParam = new SqlParameter("@CheckInDate", checkInDate.ToDateTime(TimeOnly.MinValue));
            var checkOutParam = new SqlParameter("@CheckOutDate", checkOutDate.ToDateTime(TimeOnly.MinValue));
            var guestCountParam = new SqlParameter("@GuestCount", guestCount);
            var paymentParam = new SqlParameter("@PaymentMethod", paymentMethod);

            var reservationIdParam = new SqlParameter("@ReservationId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            var resultMessageParam = new SqlParameter("@ResultMessage", System.Data.SqlDbType.NVarChar, 200)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_MakeReservation @UserId, @HotelRoomId, @CheckInDate, @CheckOutDate, @GuestCount, " +
                    "@PaymentMethod, @ReservationId OUTPUT, @ResultMessage OUTPUT",
                    userIdParam,
                    hotelRoomParam,
                    checkInParam,
                    checkOutParam,
                    guestCountParam,
                    paymentParam,
                    reservationIdParam,
                    resultMessageParam);

            var resultMessage = resultMessageParam.Value?.ToString();

            if (reservationIdParam.Value == null || reservationIdParam.Value == DBNull.Value)
            {
                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(resultMessage)
                   ? "Rezervasyon oluşturulamadı."
                    : resultMessage);
            }

            var reservationId = Convert.ToInt32(reservationIdParam.Value);

            if (reservationId <= 0)
            {
                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(resultMessage)
                    ? "Rezervasyon oluşturulamadı."
                    : resultMessage);
            }

                var reservation = await _dbSet
                       .FirstOrDefaultAsync(x => x.Id == reservationId);

            if (reservation == null)
                throw new InvalidOperationException("Rezervasyon oluşturuldu fakat kayıt geri okunamadı.");

            return reservation;
        }

        /// <summary>
        /// Check-out tarihi gecmis Confirmed rezervasyonlari Completed durumuna toplu olarak gunceller.
        /// </summary>
        /// <returns>Guncellenen rezervasyon kaydi sayisini doner.</returns>
        public async Task<int> MarkPastConfirmedAsCompletedAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _dbSet
                .Where(r => r.Status == "Confirmed" && r.CheckOutDate < today)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(r => r.Status, "Completed"));
        }

        /// <summary>
        /// Belirtilen kullanicinin belirtilen otelde tamamlanmis veya check-out tarihi gecmis konaklamasi olup olmadigini kontrol eder.
        /// </summary>
        /// <param name="userId">Kontrol edilecek kullanicinin Id degeri.</param>
        /// <param name="hotelId">Kontrol edilecek otelin Id degeri.</param>
        /// <returns>Kullanici ilgili otelde konaklamayi tamamlamissa true, aksi halde false doner.</returns>
        public async Task<bool> HasCompletedStayAsync(int userId, int hotelId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.VwReservationDetails
                .AnyAsync(v => v.UserId == userId
                            && v.HotelId == hotelId
                            && (v.Status == "Completed"
                                || (v.Status == "Confirmed" && v.CheckOutDate < today)));
        }
    }
}
