using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

public class UserRegisterDto
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email zorunludur.")]
    [EmailAddress(ErrorMessage = "Gecerli bir email giriniz!")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Þifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Þifre en az 6 karakter olmalýdýr.")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Telefon zorunludur")]
    [Phone(ErrorMessage = "Geçerli bir telefon giriniz!")]
    public string Phone { get; set; } = null!;
}
