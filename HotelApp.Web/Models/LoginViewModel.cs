using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// LoginViewModel sınıfının amacı, login formundaki veriyi taşımaktır.
// Email, şifre, beni hatırla ve dönüş URL bilgisini validation kurallarıyla view tarafına bağlar.
// SOLID: SRP kullanır; sadece login form verisini temsil eder.

public class LoginViewModel
{
    [Required(ErrorMessage = "Email zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string? Password { get; set; }

    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
