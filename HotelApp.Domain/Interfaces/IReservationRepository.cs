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
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);
        Task<Reservation?> GetWithDetailsAsync(int id);
        Task<Reservation> MakeReservationAsync(
            int hotelRoomId,
            int userId,
            DateOnly checkInDate,
            DateOnly checkOutDate,
            int guestCount,
            string paymentMethod);
        Task<IEnumerable<VwReservationDetail>> GetReservationDetailsByUserIdAsync(int userId);

        // Check-out tarihi gecmis Confirmed rezervasyonlari tek SQL UPDATE'i ile Completed yapar.
        // Donus: etkilenen kayit sayisi. Periyodik calistirma icin Background Service tarafindan cagrilir.
        Task<int> MarkPastConfirmedAsCompletedAsync();
    }
}
