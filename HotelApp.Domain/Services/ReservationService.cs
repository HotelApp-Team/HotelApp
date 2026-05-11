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
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId)
        {
            var reservations = await _unitOfWork.ReservationRepository.GetByUserIdAsync(userId);

            return reservations.Select(MapToDto);

        }

        public async Task<IEnumerable<ReservationDetailDto>> GetUserReservationDetailsAsync(int userId)
        {
            var details = await _unitOfWork.ReservationRepository
                .GetReservationDetailsByUserIdAsync(userId);

            return details.Select(x => new ReservationDetailDto
            {
                ReservationId = x.ReservationId,
                HotelName = x.HotelName,
                RoomTypeName = x.RoomTypeName,
                CheckInDate = x.CheckInDate,
                CheckOutDate = x.CheckOutDate,
                GuestCount = x.GuestCount,
                TotalPrice = x.TotalPrice,
                Status = x.Status
            });
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetWithDetailsAsync(id);

            if (reservation == null)
                return null;

            return MapToDto(reservation);
        }


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

        // Check-out tarihi gecmis Confirmed rezervasyonlari Completed yapan toplu UPDATE'i tetikler.
        // Repository ExecuteUpdateAsync kullaniyor; SaveChanges cagirmaya gerek yok.
        // Donus: etkilenen kayit sayisi (loglama/observability icin).
        public async Task<int> MarkPastReservationsCompletedAsync()
        {
            return await _unitOfWork.ReservationRepository.MarkPastConfirmedAsCompletedAsync();
        }

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
                HotelName = reservation.HotelRoom?.Hotel?.Name ?? string.Empty,
                RoomTypeName = reservation.HotelRoom?.RoomType?.Name ?? string.Empty,
                CreatedAt = reservation.CreatedAt
            };
        }


    }
}
