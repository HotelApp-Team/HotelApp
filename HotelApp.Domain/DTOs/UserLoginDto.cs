using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// UserLoginDto sınıfının amacı, kullanıcı giriş isteğini taşımaktır.
// Email, şifre ve beni hatırla bilgisini validation kurallarıyla API tarafına aktarır.
// SOLID: SRP kullanır; sadece login istek verisini temsil eder.

public class UserLoginDto
{
    [Required(ErrorMessage = "Email zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; } = false;
}