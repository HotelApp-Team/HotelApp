namespace HotelApp.Domain.Services;

// IEmailService arayüzünün amacı, email gönderme davranışı için sözleşme tanımlamaktır.
// Doğrulama maili gönderme ve güvenli gönderim denemesi metotlarını belirler.
// SOLID: ISP kullanır; sadece email servis ihtiyaçlarını içerir.
// SOLID: DIP sağlar; auth servisi somut mail altyapısı yerine bu arayüze bağlı kalır.

public interface IEmailService
{
    // Kullaniciya email dogrulama maili gonderir.
    // Hata olursa exception firlatir; teknik detay isteyen akislarda kullanilir.
    Task SendVerificationEmailAsync(string toEmail, string firstName, string token);

    // Mail gonderimini "best effort" dener.
    // Auth akisinin kirilmamasi icin exception yerine basari/basarisizlik sonucu doner.
    Task<bool> SendVerificationEmailSafelyAsync(string toEmail, string firstName, string token);
}
