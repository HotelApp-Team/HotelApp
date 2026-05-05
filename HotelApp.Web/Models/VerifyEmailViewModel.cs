namespace HotelApp.Web.Models;

// VerifyEmailViewModel sınıfının amacı, email doğrulama sayfasının durum bilgisini taşımaktır.
// Başarı/hata durumunu, kullanıcı mesajını ve tekrar mail gönderme için email bilgisini view'a aktarır.
// SOLID: SRP kullanır; sadece email doğrulama sayfa verisini temsil eder.

public class VerifyEmailViewModel
{
    // Dogrulama basarili mi?
    public bool IsSuccess { get; set; }

    // Kullaniciya gosterilecek mesaj
    public string Message { get; set; } = string.Empty;

    // Resend (tekrar mail gonder) butonu icin email - Madde 7'de kullanilacak
    public string? Email { get; set; }
}
