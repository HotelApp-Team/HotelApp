namespace HotelApp.Domain.DTOs;

// AdminUserDto sınıfının amacı, admin panelinde gösterilecek kullanıcı bilgisini taşımaktır.
// Kullanıcının temel bilgilerini, aktiflik durumunu ve rollerini güvenli DTO olarak döndürür.
// SOLID: SRP kullanır; sadece admin kullanıcı cevap verisini temsil eder.

public class AdminUserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
