using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// Kullanici "maili tekrar gonder" dediginde API'ye giden istektir.
public class ResendVerificationDto
{
    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    public string Email { get; set; } = null!;
}

// Resend isleminin controller tarafinda loglanabilmesi icin ic sonuc bilgisini tasir.
// Disariya yine generic message gosterilir; EmailSent / EmailSendAttempted daha cok ic akis icindir.
public class ResendVerificationResultDto
{
    public string Message { get; set; } = string.Empty;
    public bool EmailSent { get; set; }
    public bool EmailSendAttempted { get; set; }
}
