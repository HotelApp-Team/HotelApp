namespace HotelApp.Web.Models;

// ErrorPageViewModel sınıfının amacı, /Home/Error sayfasinda
// status koduna gore baslik ve mesaj gostermektir.
// 404/403/401/500 gibi HTTP durum kodlari icin kullaniciya nazik metin tasinir.
// SOLID: SRP kullanir; sadece hata sayfasinin ekran verisini temsil eder.
public class ErrorPageViewModel
{
    // Gosterilecek HTTP durum kodu (404, 403, 500 vb.)
    public int StatusCode { get; set; }

    // Sayfa baslik metni (orn. "Sayfa bulunamadi").
    public string Title { get; set; } = "Bir şeyler ters gitti";

    // Aciklama metni - kullaniciya ne oldugunu sade dilde anlatir.
    public string Message { get; set; } = "Beklenmeyen bir hata oluştu.";
}
