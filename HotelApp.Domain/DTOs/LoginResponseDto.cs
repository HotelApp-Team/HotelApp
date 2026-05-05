namespace HotelApp.Domain.DTOs;

// LoginResponseDto sınıfının amacı, başarılı giriş sonucunu taşımaktır.
// JWT token, kullanıcı emaili, tam ad ve token geçerlilik süresini API cevabı olarak döndürür.
// SOLID: SRP kullanır; sadece login cevap verisini temsil eder.

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
