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
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(HotelAppDbContext context) : base(context)
        {
        }

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

        public async Task<IEnumerable<VwReservationDetail>> GetReservationDetailsByUserIdAsync(int userId)
        {
            return await _context.VwReservationDetails
                 .Where(x => x.UserId == userId)
                 .OrderByDescending(x => x.CreatedAt)
                 .ToListAsync();
        }

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
    }
}
