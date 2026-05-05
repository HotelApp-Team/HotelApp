namespace HotelApp.Domain.DTOs;

// UserDto sınıfının amacı, kullanıcı profil bilgisini API cevabı olarak taşımaktır.
// Kullanıcının güvenli şekilde gösterilebilecek temel bilgilerini ve rollerini dışarıya aktarır.
// SOLID: SRP kullanır; sadece kullanıcı cevap verisini temsil eder.

public class UserDto
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
