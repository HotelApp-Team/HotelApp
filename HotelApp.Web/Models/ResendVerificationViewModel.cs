using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// "Tekrar doğrulama maili gönder" formunun taşıdığı tek alan: Email.
// Backend'in ResendVerificationDto'su ile aynı yapıda
public class ResendVerificationViewModel
{
    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }
}
