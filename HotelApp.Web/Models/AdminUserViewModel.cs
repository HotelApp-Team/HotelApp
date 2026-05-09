namespace HotelApp.Web.Models;

// AdminUserViewModel sınıfının amacı, E37-E39 kullanıcı yönetimi verisini admin users ekranına taşımaktır.
// Kullanıcı kimlik, iletişim, aktiflik ve rol bilgilerini içerir; hassas password/token alanları içermez.
// SOLID: SRP kullanır; sadece admin kullanıcı satırı görüntüleme verisini temsil eder.
public class AdminUserViewModel
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public List<string>? Roles { get; set; }
    public DateTime CreatedAt { get; set; }
}
