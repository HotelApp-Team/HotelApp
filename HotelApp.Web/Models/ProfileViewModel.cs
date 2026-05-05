using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// ProfileViewModel sınıfının amacı, web profil formunun verisini taşımaktır.
// Ad, soyad, email ve telefon bilgisini validation kurallarıyla view tarafına bağlar.
// SOLID: SRP kullanır; sadece profil form verisini temsil eder.

public class ProfileViewModel
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [Display(Name = "Telefon")]
    public string Phone { get; set; } = null!;
}
