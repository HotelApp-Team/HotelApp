using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// ResendVerificationDto sınıfının amacı, doğrulama mailini tekrar gönderme isteğini taşımaktır.
// Kullanıcının email bilgisini validation kurallarıyla birlikte API tarafına aktarır.
// SOLID: SRP kullanır; sadece resend verification istek verisini temsil eder.

public class ResendVerificationDto
{
    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    public string Email { get; set; } = null!;
}

// ResendVerificationResultDto sınıfının amacı, doğrulama maili tekrar gönderme sonucunu taşımaktır.
// Controller'ın log seviyesini seçebilmesi için mesaj, mail denendi mi ve gönderildi mi bilgilerini tutar.
// SOLID: SRP kullanır; sadece resend verification iç sonuç verisini temsil eder.

public class ResendVerificationResultDto
{
    public string Message { get; set; } = string.Empty;
    public bool EmailSent { get; set; }
    public bool EmailSendAttempted { get; set; }
}
