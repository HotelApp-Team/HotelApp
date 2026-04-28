namespace HotelApp.Web.Models;

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
