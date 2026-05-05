using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// RegisterViewModel sınıfının amacı, kayıt formundaki veriyi taşımaktır.
// Ad, soyad, email, şifre, şifre tekrarı ve telefon bilgisini validation kurallarıyla view tarafına bağlar.
// SOLID: SRP kullanır; sadece register form verisini temsil eder.

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Soyad")]
    public string? LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
    [Display(Name = "Email")]
    public string? Email { get; set; } = null!;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string? Password { get; set; } = null!;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre Tekrar")]
    public string? ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "Telefon zorunludur")]
    [Phone(ErrorMessage = "Geçerli bir telefon giriniz!")]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; } = null!;
}
