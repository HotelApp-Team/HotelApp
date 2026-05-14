using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// ForgotPasswordDto sınıfının amacı, şifre yenileme linki isteyen kullanıcının email bilgisini taşımaktır.
// Email validasyonu ile API tarafına temiz istek verisi aktarır.
// SOLID: SRP kullanır; sadece şifremi unuttum istek verisini temsil eder.

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    public string Email { get; set; } = null!;
}

// ForgotPasswordResultDto sınıfının amacı, şifre yenileme linki isteme sonucunu taşımaktır.
// Dışarıya generic mesaj dönerken controller'ın log kararını verebilmesi için iç durum bilgisini tutar.
// SOLID: SRP kullanır; sadece forgot password işlem sonucunu temsil eder.

public class ForgotPasswordResultDto
{
    public string Message { get; set; } = string.Empty;
    public bool EmailSendAttempted { get; set; }
}

// ResetPasswordDto sınıfının amacı, reset token ile yeni şifre belirleme isteğini taşımaktır.
// Token, yeni şifre ve tekrar alanlarını validation kurallarıyla API tarafına aktarır.
// SOLID: SRP kullanır; sadece şifre yenileme istek verisini temsil eder.

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Token alanı zorunludur.")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Yeni şifrenin doğrulaması zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler uyuşmuyor.")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; } = null!;
}

// ResetPasswordResultDto sınıfının amacı, başarılı şifre yenileme sonucundaki log bilgisini taşımaktır.
// Controller'ın kullanıcı emaili ile bilgi logu atabilmesi için kullanılır.
// SOLID: SRP kullanır; sadece reset password işlem sonucunu temsil eder.

public class ResetPasswordResultDto
{
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
