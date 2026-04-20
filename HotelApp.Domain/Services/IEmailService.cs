// IEmailService, email gonderme davranisinin sozlesmesini tanimlar.
// Amaci, farkli mail altyapilarini kolayca degistirebilmektir.
// SOLID: DIP ve ISP kullanir.
namespace HotelApp.Domain.Services;

public interface IEmailService
{
    // Kullaniciya email dogrulama maili gonderir
    Task SendVerificationEmailAsync(string toEmail, string firstName, string token);
}
