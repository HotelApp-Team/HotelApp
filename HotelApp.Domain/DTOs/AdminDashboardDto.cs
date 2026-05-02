namespace HotelApp.Domain.DTOs;

// Amaç: Admin dashboard ekranında gösterilecek özet istatistikleri taşır.
// SOLID: SRP ile sadece dashboard cevap verisini temsil eder.

public class AdminDashboardDto
{
    public int TotalHotels { get; set; }
    public int TotalUsers { get; set; }
    public int TotalReservations { get; set; }
    public int ActiveReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
    public List<ReservationDto> RecentReservations { get; set; } = new();
}
