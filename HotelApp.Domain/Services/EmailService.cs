using System.Net;
using System.Net.Mail;

namespace HotelApp.Domain.Services;

// EmailService sınıfının amacı, kullanıcı email doğrulama mailini göndermektir.
// SMTP ayarlarını kullanarak doğrulama linki hazırlar ve mail gönderme işlemini yapar.
// SOLID: SRP kullanır; sadece email gönderme işleminden sorumludur.
// SOLID: DIP destekler; üst katmanlar bu sınıfı IEmailService arayüzü üzerinden kullanır.

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

    // Sifre yenileme linkinin temel adresi
    private readonly string _resetPasswordBaseUrl;

    // Constructor ile appsettings.json'dan gelen EmailSettings degerlerini aliyoruz
    public EmailService(string smtpServer, int smtpPort, string senderEmail, string senderName, string username, string password, string verifyEmailBaseUrl, string resetPasswordBaseUrl)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _senderEmail = senderEmail;
        _senderName = senderName;
        _username = username;
        _password = password;
        _verifyEmailBaseUrl = verifyEmailBaseUrl;
        _resetPasswordBaseUrl = resetPasswordBaseUrl;
    }

    // E01 ve E43 için doğrulama mailini SMTP üzerinden gönderir.
    public async Task SendVerificationEmailAsync(string toEmail, string firstName, string token)
    {
        // Token ile dogrulama linkini olustur
        var verificationLink = $"{_verifyEmailBaseUrl}?token={Uri.EscapeDataString(token)}";

        // Mail konusu
        var subject = "HotelApp Email Dogrulama";

        // Mail icerigi
        var body = $@"
        Merhaba {firstName},

        Hesabınızı doğrulamak için aşağıdaki linke tıklayın:
        {verificationLink}

        Eğer bu işlemi siz yapmadıysanız bu maili dikkate almayın.";

        using var message = new MailMessage
        {
            From = new MailAddress(_senderEmail, _senderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
            BodyEncoding = System.Text.Encoding.UTF8,
            SubjectEncoding = System.Text.Encoding.UTF8
        };

        message.To.Add(toEmail);

        using var client = new SmtpClient(_smtpServer, _smtpPort)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }

    // E01 ve E43 için mail gönderimini dener, hata olursa false döner.
    public async Task<bool> SendVerificationEmailSafelyAsync(string toEmail, string firstName, string token)
    {
        try
        {
            await SendVerificationEmailAsync(toEmail, firstName, token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // E54 için şifre yenileme linkini SMTP üzerinden gönderir.
    public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string token)
    {
        // Token ile sifre yenileme linkini olusturur.
        var resetLink = $"{_resetPasswordBaseUrl}?token={Uri.EscapeDataString(token)}";

        // Mail konusu
        var subject = "HotelApp Sifre Yenileme";

        // Mail icerigi
        var body = $@"
        Merhaba {firstName},

        Sifrenizi yenilemek icin asagidaki linke tiklayin:
        {resetLink}

        Bu link 30 dakika gecerlidir.
        Eger bu islemi siz yapmadiysaniz bu maili dikkate almayin.";

        using var message = new MailMessage
        {
            From = new MailAddress(_senderEmail, _senderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
            BodyEncoding = System.Text.Encoding.UTF8,
            SubjectEncoding = System.Text.Encoding.UTF8
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
