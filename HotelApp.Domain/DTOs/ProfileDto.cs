using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

public class ProfileDto
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Telefon zorunludur")]
    [Phone(ErrorMessage = "Gecerli bir telefon giriniz!")]
    public string Phone { get; set; } = null!;
}
