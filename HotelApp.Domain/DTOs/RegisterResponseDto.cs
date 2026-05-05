namespace HotelApp.Domain.DTOs;

// RegisterResponseDto sınıfının amacı, kayıt işleminin sonucunu taşımaktır.
// Email doğrulama gerekip gerekmediğini, mesajı, token bilgisini ve mail gönderim sonucunu döndürür.
// SOLID: SRP kullanır; sadece register cevap verisini temsil eder.

public class RegisterResponseDto
{
    public bool RequiresEmailVerification { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Disa acik kullanici mesaji zaten ayri donuyor; bu alan daha cok API/controller log kararinda kullanilir.
    public bool VerificationEmailSent { get; set; }
}
