namespace HotelApp.Web.Models;

// AdminDashboardViewModel sınıfının amacı, E36 dashboard istatistiklerini admin dashboard view'ına taşımaktır.
// Toplam sayılar, gelir, ortalama puan ve son rezervasyonlar tek model üzerinden render edilir.
// SOLID: SRP kullanır; sadece dashboard ekranının veri taşıma sorumluluğunu üstlenir.
public class AdminDashboardViewModel
{
    public int TotalHotels { get; set; }
    public int TotalUsers { get; set; }
    public int TotalReservations { get; set; }
    public int ActiveReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
    public List<RecentReservationViewModel>? RecentReservations { get; set; }
}
