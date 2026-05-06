namespace HotelApp.Domain.DTOs;

// AdminDashboardDto sınıfının amacı, admin dashboard ekranının özet verilerini taşımaktır.
// Toplam otel, kullanıcı, rezervasyon, gelir ve son rezervasyon bilgilerini API cevabı olarak hazırlar.
// SOLID: SRP kullanır; sadece dashboard cevap verisini temsil eder.

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
