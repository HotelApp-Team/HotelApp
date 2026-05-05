using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// ProfileDto sınıfının amacı, kullanıcının profil güncelleme verisini taşımaktır.
// Ad, soyad ve telefon alanlarını validation kurallarıyla birlikte API tarafına aktarır.
// SOLID: SRP kullanır; sadece profil istek verisini temsil eder.

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
