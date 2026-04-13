using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

public class UserLoginDto
{
    [Required(ErrorMessage = "Email zorunludur.")]
    [EmailAddress(ErrorMessage = "Gecerli bir email giriniz!")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Sifre zorunludur.")]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; } = false;
}