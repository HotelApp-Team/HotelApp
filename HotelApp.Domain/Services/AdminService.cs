using HotelApp.Domain.DTOs;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services;

// Amaç: Admin dashboard verilerini repository katmanından alır ve API'nin döneceği DTO'ya hazırlar.
// SOLID: SRP ile sadece admin dashboard iş akışından sorumludur; DIP ile IUnitOfWork arayüzüne bağlı çalışır.

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    // UnitOfWork bağımlılığını alır.
    public AdminService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // E36 için dashboard istatistiklerini toplar ve tek DTO halinde hazırlar.
    public async Task<AdminDashboardDto> GetDashboardDataAsync()
    {
        // Son 10 rezervasyonu veritabanından alıyoruz.
        var recentReservations = await _unitOfWork.AdminRepository
            .GetRecentReservationsAsync(10);

        // Repository veriyi getirir, service ise API cevabını hazırlar.
        return new AdminDashboardDto
        {
            TotalHotels = await _unitOfWork.AdminRepository.GetTotalHotelsAsync(),
            TotalUsers = await _unitOfWork.AdminRepository.GetTotalUsersAsync(),
            TotalReservations = await _unitOfWork.AdminRepository.GetTotalReservationsAsync(),
            ActiveReservations = await _unitOfWork.AdminRepository.GetActiveReservationsAsync(),
            TotalRevenue = await _unitOfWork.AdminRepository.GetTotalRevenueAsync(),
            AverageRating = await _unitOfWork.AdminRepository.GetAverageRatingAsync(),

            // Reservation entity listesini API'de dönecek ReservationDto listesine çeviriyoruz.
            RecentReservations = recentReservations
                .Select(ReservationService.MapToDto)
                .ToList()
        };
    }
}
