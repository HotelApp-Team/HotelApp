using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// ResendVerificationViewModel sınıfının amacı, doğrulama mailini tekrar gönderme formunun verisini taşımaktır.
// Kullanıcının email bilgisini validation kurallarıyla view tarafına bağlar.
// SOLID: SRP kullanır; sadece resend verification form verisini temsil eder.

public class ResendVerificationViewModel
{
    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }
}
