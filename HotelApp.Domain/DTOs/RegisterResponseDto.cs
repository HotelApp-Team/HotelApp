namespace HotelApp.Domain.DTOs;

// Register sonucunu tasir.
// Faz 2'de email dogrulama bekleniyorsa token yerine durum bilgisi doner.
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
