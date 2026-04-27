namespace HotelApp.Web.Models;

// VerifyEmail sayfasinin durumunu  (basari/hata) view'a tasiyan kucuk model
public class VerifyEmailViewModel
{
    // Dogrulama basarili mi?
    public bool IsSuccess { get; set; }

    // Kullaniciya gosterilecek mesaj
    public string Message { get; set; } = string.Empty;

    // Resend (tekrar mail gonder) butonu icin email - Madde 7'de kullanilacak
    public string? Email { get; set; }
}
