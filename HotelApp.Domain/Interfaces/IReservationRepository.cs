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

        // Kullanicinin verilen otelde tamamlanmis konaklamasi var mi?
        // Yorum yazma yetkisi icin ReviewService tarafindan cagrilir.
        // Esnek tanim: Status="Completed" VEYA (Status="Confirmed" AND CheckOutDate gecmis).
        // Boylece background service henuz Completed'a cevirmemis olsa bile
        // konaklamasi biten kullanici yorum yazabilir.
        Task<bool> HasCompletedStayAsync(int userId, int hotelId);
    }
}
