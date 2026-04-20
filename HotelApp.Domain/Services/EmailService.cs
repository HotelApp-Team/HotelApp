using System.Net;
using System.Net.Mail;

// EmailService, email dogrulama mailini teknik olarak SMTP uzerinden gonderir.
// Amaci, mail gonderme detaylarini auth akisindan ayirmaktir.
// SOLID: SRP kullanir, DIP sayesinde IEmailService uzerinden degistirilebilir.
namespace HotelApp.Domain.Services;

public class EmailService : IEmailService
{
    // SMTP sunucu adresi
    private readonly string _smtpServer;

    // SMTP port numarasi
    private readonly int _smtpPort;

    // Mailin gonderilecegi email adresi
    private readonly string _senderEmail;

    // Gonderici olarak gorunecek isim
    private readonly string _senderName;

    // SMTP kullanici adi
    private readonly string _username;

    // SMTP sifresi
    private readonly string _password;

    // Dogrulama linkinin temel adresi
    private readonly string _verifyEmailBaseUrl;

    // Constructor ile appsettings.json'dan gelen EmailSettings degerlerini aliyoruz
    public EmailService(string smtpServer, int smtpPort, string senderEmail, string senderName, string username, string password, string verifyEmailBaseUrl)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _senderEmail = senderEmail;
        _senderName = senderName;
        _username = username;
        _password = password;
        _verifyEmailBaseUrl = verifyEmailBaseUrl;
    }

    public async Task SendVerificationEmailAsync(string toEmail, string firstName, string token)
    {
        // Token ile dogrulama linkini olustur
        var verificationLink = $"{_verifyEmailBaseUrl}?token={Uri.EscapeDataString(token)}";

        // Mail konusu
        var subject = "HotelApp Email Dogrulama";

        // Mail icerigi
        var body = $@"
        Merhaba {firstName},

        Hesabýnýzý doðrulamak için aþaðýdaki linke týklayýn:
        {verificationLink}

        Eðer bu iþlemi siz yapmadýysanýz bu maili dikkate almayýn.";

        using var message = new MailMessage
        {
            From = new MailAddress(_senderEmail, _senderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        message.To.Add(toEmail);

        using var client = new SmtpClient(_smtpServer, _smtpPort)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
