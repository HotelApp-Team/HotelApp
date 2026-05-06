namespace HotelApp.Web.Models;

// RegisterResponse sınıfının amacı, API'den gelen kayıt sonucunu web tarafında taşımaktır.
// Email doğrulama durumu, mesaj, token ve kullanıcı bilgilerini register akışında kullanır.
// SOLID: SRP kullanır; sadece web register cevap verisini temsil eder.

public class RegisterResponse
{
    // API'de dönen başarılı kayıt yanıtını taşır
    // Faz 1'de Token+Email+FullName+ExpiresAt dolu (otomatik giriş)
    // Faz 2'de bu alanlar null, RequiresEmailVerification=true (email doğrulmaa gerek)

    public bool RequiresEmailVerification { get; set; }
    public string Message { get; set; } = null!;
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool VerificationEmailSent { get; set; }
}
