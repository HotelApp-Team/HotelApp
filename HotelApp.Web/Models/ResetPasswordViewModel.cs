using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// ResetPasswordViewModel sınıfının amacı, reset token ile yeni şifre belirleme formunun verisini taşımaktır.
// Token, yeni şifre, şifre tekrarı ve sayfa mesajlarını validation kurallarıyla view tarafına aktarır.
// SOLID: SRP kullanır; sadece şifre yenileme sayfa verisini temsil eder.

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Şifre yenileme linki geçersiz veya süresi dolmuş.")]
    public string? Token { get; set; }

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Yeni şifrenin doğrulaması zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler uyuşmuyor.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre Tekrar")]
    public string? ConfirmNewPassword { get; set; }

    // Link hataliysa form yerine uyarı göstermek için kullanılır.
    public bool IsInvalidLink { get; set; }

    // Kullanıcıya gösterilecek hata veya bilgilendirme mesajını taşır.
    public string? Message { get; set; }

    // Auth ekranindaki sol gorsel alaninda gosterilecek hero istatistiklerini tasir.
    public HeroStatsViewModel? HeroStats { get; set; }
}

// ResetPasswordResponse sınıfının amacı, API'nin reset-password yanıtındaki mesajı web tarafında taşımaktır.
// SOLID: SRP kullanır; sadece API mesaj cevabını temsil eder.

public class ResetPasswordResponse
{
    public string Message { get; set; } = string.Empty;
}
