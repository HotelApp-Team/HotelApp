// IEmailService, email gonderme davranisinin sozlesmesini tanimlar.
// Amaci, farkli mail altyapilarini kolayca degistirebilmektir.
// SOLID: DIP ve ISP kullanir.
namespace HotelApp.Domain.Services;

public interface IEmailService
{
    // Kullaniciya email dogrulama maili gonderir.
    // Hata olursa exception firlatir; teknik detay isteyen akislarda kullanilir.
    Task SendVerificationEmailAsync(string toEmail, string firstName, string token);

    // Mail gonderimini "best effort" dener.
    // Auth akisinin kirilmamasi icin exception yerine basari/basarisizlik sonucu doner.
    Task<bool> SendVerificationEmailSafelyAsync(string toEmail, string firstName, string token);
}
