using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// ChangePasswordViewModel sınıfının amacı, web şifre değiştirme formunun verisini taşımaktır.
// Mevcut şifre, yeni şifre ve yeni şifre tekrar alanlarını validation kurallarıyla view tarafına bağlar.
// SOLID: SRP kullanır; sadece şifre değiştirme form verisini temsil eder.

public class ChangePasswordViewModel
{

    [Required(ErrorMessage = "Sifre zorunludur.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mevcut Şifre")]
    public string CurrentPassword { get; set; } = null!; // Eski şifre, zorunlu alan

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    public string NewPassword { get; set; } = null!; // Yeni şifre, zorunlu alan

    [Required(ErrorMessage = "Yeni şifrenin doğrulaması zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre Tekrar")]
    public string ConfirmNewPassword { get; set; } = null!; // Yeni şifrenin doğrulaması, zorunlu alan
}
