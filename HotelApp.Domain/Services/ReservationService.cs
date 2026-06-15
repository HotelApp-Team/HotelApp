using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    /// <summary>
    /// Rezervasyon islemlerine ait is kurallarini yoneten servis sinifidir.
    /// Kullanici rezervasyonlarini listeleme, rezervasyon olusturma, iptal etme
    /// ve gecmis rezervasyonlari tamamlama islemlerini gerceklestirir.
    /// </summary>
    public class ReservationService : IReservationService
    {
        /// <summary>
        /// ReservationService sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="unitOfWork">Repository islemlerine erisim icin kullanilan UnitOfWork nesnesi.</param>
        private readonly IUnitOfWork _unitOfWork;

        public ReservationService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        /// <summary>
        /// Belirtilen kullaniciya ait rezervasyonlari getirir.
        /// </summary>
        /// <param name="userId">Rezervasyonlari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullanicinin rezervasyon DTO listesini doner.</returns>
        public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId)
        {
            var reservations = await _unitOfWork.ReservationRepository.GetByUserIdAsync(userId);

            return reservations.Select(MapToDto);

        }

        /// <summary>
        /// Belirtilen kullaniciya ait rezervasyon detaylarini view uzerinden getirir.
        /// </summary>
        /// <param name="userId">Rezervasyon detaylari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullanicinin rezervasyon detay DTO listesini doner.</returns>
        public async Task<IEnumerable<ReservationDetailDto>> GetUserReservationDetailsAsync(int userId)
        {
            var details = await _unitOfWork.ReservationRepository
                .GetReservationDetailsByUserIdAsync(userId);

            return details.Select(x => new ReservationDetailDto
            {
                ReservationId = x.ReservationId,
                // VwReservationDetail view'inde HotelId zaten projeksiyon olarak geliyor;
                // dogrudan tasiyoruz, ek sorgu yok.
                HotelId = x.HotelId,
                HotelName = x.HotelName,
                MainImageUrl = x.MainImageUrl,
                RoomTypeName = x.RoomTypeName,
                CheckInDate = x.CheckInDate.ToDateTime(TimeOnly.MinValue),
                CheckOutDate = x.CheckOutDate.ToDateTime(TimeOnly.MinValue),
                GuestCount = x.GuestCount,
                TotalPrice = x.TotalPrice,
                Status = x.Status
            });
        }

        /// <summary>
        /// Verilen Id degerine sahip rezervasyonu detay bilgileriyle birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek rezervasyonun Id degeri.</param>
        /// <returns>Rezervasyon DTO nesnesi; bulunamazsa null.</returns>
        public async Task<ReservationDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetWithDetailsAsync(id);

            if (reservation == null)
                return null;

            return MapToDto(reservation);
        }

        /// <summary>
        /// Belirtilen kullanici icin yeni rezervasyon olusturur.
        /// Tarih, misafir sayisi, odeme yontemi, oda aktifligi ve kapasite kontrollerini yapar.
        /// </summary>
        /// <param name="userId">Rezervasyonu olusturan kullanicinin Id degeri.</param>
        /// <param name="dto">Rezervasyon olusturma bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan rezervasyon DTO nesnesini doner.</returns>
        /// <exception cref="InvalidOperationException">Validasyon veya is kurali ihlali durumunda firlatilir.</exception>
        /// <exception cref="KeyNotFoundException">Oda bulunamazsa veya aktif degilse firlatilir.</exception>
        public async Task<ReservationDto> CreateReservationAsync(int userId, CreateReservationDto dto)
        {
            if (dto.CheckInDate < DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Giriş tarihi bugünden önce olamaz.");

            if (dto.CheckInDate >= dto.CheckOutDate)
                throw new InvalidOperationException("Giriş tarihi çıkış tarihinden küçük olmalıdır.");

            if (dto.GuestCount <= 0)
                throw new InvalidOperationException("Misafir sayısı en az 1 olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
                throw new InvalidOperationException("Ödeme yöntemi zorunludur.");

            var room = await _unitOfWork.HotelRoomRepository.GetRoomWithDetailsAsync(dto.HotelRoomId);

            if (room == null)
                throw new KeyNotFoundException("Oda bulunamadı.");

            if (!room.IsActive)
                throw new KeyNotFoundException("Oda aktif değil.");

            if (room.RoomType.MaxGuests < dto.GuestCount)
                throw new InvalidOperationException("Misafir sayısı bu oda tipi için fazla.");

            var reservation = await _unitOfWork.ReservationRepository.MakeReservationAsync(
                dto.HotelRoomId,
                userId,
                dto.CheckInDate,
                dto.CheckOutDate,
                dto.GuestCount,
                dto.PaymentMethod);

            var reservationWithDetails = await _unitOfWork.ReservationRepository.GetWithDetailsAsync(reservation.Id);

            if (reservationWithDetails == null)
                throw new InvalidOperationException("Rezervasyon oluşturuldu fakat detayları geri okunamadı.");

            return MapToDto(reservationWithDetails);
        }

        /// <summary>
        /// Belirtilen rezervasyonu iptal eder.
        /// Kullanici yalnizca kendi rezervasyonunu iptal edebilir; admin tum rezervasyonlari iptal edebilir.
        /// </summary>
        /// <param name="id">Iptal edilecek rezervasyonun Id degeri.</param>
        /// <param name="userId">Islemi yapan kullanicinin Id degeri.</param>
        /// <param name="isAdmin">Islemi yapan kullanicinin admin olup olmadigini belirtir.</param>
        /// <exception cref="KeyNotFoundException">Rezervasyon bulunamazsa firlatilir.</exception>
        /// <exception cref="UnauthorizedAccessException">Kullanicinin iptal yetkisi yoksa firlatilir.</exception>
        /// <exception cref="InvalidOperationException">Rezervasyon durumu iptal icin uygun degilse firlatilir.</exception>
        public async Task CancelReservationAsync(int id, int userId, bool isAdmin)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(id);

            if (reservation == null)
                throw new KeyNotFoundException("Rezervasyon bulunamadı.");

            if (reservation.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("Bu rezervasyonu iptal etme yetkiniz yok.");

            var status = reservation.Status?.Trim();

            if (string.IsNullOrWhiteSpace(status))
                throw new InvalidOperationException("Rezervasyon durumu geçersiz.");

            if (status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Bu rezervasyon zaten iptal edilmiş.");

            if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Tamamlanmış rezervasyon iptal edilemez.");

            if (reservation.CheckInDate < DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Geçmiş tarihli rezervasyon iptal edilemez.");

            reservation.Status = "Cancelled";

            _unitOfWork.ReservationRepository.Update(reservation);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Check-out tarihi gecmis Confirmed rezervasyonlari Completed durumuna gunceller.
        /// Repository toplu guncelleme yaptigi icin SaveChanges cagirmaya gerek yoktur.
        /// </summary>
        /// <returns>Guncellenen rezervasyon kaydi sayisini doner.</returns>
        public async Task<int> MarkPastReservationsCompletedAsync()
        {
            return await _unitOfWork.ReservationRepository.MarkPastConfirmedAsCompletedAsync();
        }

        /// <summary>
        /// Reservation entity nesnesini ReservationDto nesnesine donusturur.
        /// </summary>
        /// <param name="reservation">Donusturulecek rezervasyon entity nesnesi.</param>
        /// <returns>ReservationDto nesnesini doner.</returns>
        public static ReservationDto MapToDto(Reservation reservation)
        {
            return new ReservationDto
            {
                Id = reservation.Id,
                HotelRoomId = reservation.HotelRoomId,
                UserId = reservation.UserId,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                GuestCount = reservation.GuestCount,
                TotalPrice = reservation.TotalPrice,
                PaymentMethod = reservation.PaymentMethod,
                Status = reservation.Status,
                UserName = $"{reservation.User?.FirstName} {reservation.User?.LastName}".Trim(),
                // HotelRoom -> Hotel zinciri uzerinden HotelId aliyoruz.
                // GetWithDetailsAsync zaten bu navigation property'leri yukluyor.
                HotelId = reservation.HotelRoom?.Hotel?.Id ?? 0,
                HotelName = reservation.HotelRoom?.Hotel?.Name ?? string.Empty,
                RoomTypeName = reservation.HotelRoom?.RoomType?.Name ?? string.Empty,
                CreatedAt = reservation.CreatedAt
            };
        }
    }
}
