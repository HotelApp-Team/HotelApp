using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// ForgotPasswordViewModel sınıfının amacı, şifremi unuttum formundaki email bilgisini taşımaktır.
// Form sonucu mesajını ve auth ekranında gösterilecek hero istatistiklerini view tarafına aktarır.
// SOLID: SRP kullanır; sadece şifremi unuttum sayfa verisini temsil eder.

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    // API'den gelen güvenli/generic bilgilendirme mesajını taşır.
    public string? Message { get; set; }

    // Form gönderildikten sonra başarı bilgilendirme kartını göstermek için kullanılır.
    public bool IsSubmitted { get; set; }

    // Auth ekranindaki sol gorsel alaninda gosterilecek hero istatistiklerini tasir.
    public HeroStatsViewModel? HeroStats { get; set; }
}

// ForgotPasswordResponse sınıfının amacı, API'nin forgot-password yanıtındaki mesajı web tarafında taşımaktır.
// SOLID: SRP kullanır; sadece API mesaj cevabını temsil eder.

public class ForgotPasswordResponse
{
    public string Message { get; set; } = string.Empty;
}
