using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// UserChangePasswordDto sınıfının amacı, kullanıcının şifre değiştirme isteğini taşımaktır.
// Mevcut şifre, yeni şifre ve yeni şifre tekrar alanlarını validation kurallarıyla API tarafına aktarır.
// SOLID: SRP kullanır; sadece şifre değiştirme istek verisini temsil eder.

public class UserChangePasswordDto
{
    [Required(ErrorMessage = "Sifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Sifre en az 6 karakter olmalidir.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = null!; // Eski şifre, zorunlu alan
    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = null!; // Yeni şifre, zorunlu alan

    [Required(ErrorMessage = "Yeni şifrenin doğrulaması zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; } = null!; // Yeni şifrenin doğrulaması, zorunlu alan
}
