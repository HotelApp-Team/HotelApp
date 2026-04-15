using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

public class ResendVerificationDto
{
    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    public string Email { get; set; } = null!;
};
