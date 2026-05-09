namespace HotelApp.Web.Models;

// RecentReservationViewModel sınıfının amacı, dashboard ekranındaki son rezervasyon satırlarını taşımaktır.
// E36 response içindeki rezervasyon özetini tabloya basmak için gerekli alanları içerir.
// SOLID: SRP kullanır; sadece son rezervasyon özet verisini temsil eder.
public class RecentReservationViewModel
{
    public int Id { get; set; }
    public string? HotelName { get; set; }
    public string? UserName { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string? Status { get; set; }
    public decimal TotalPrice { get; set; }
}
