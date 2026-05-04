namespace HotelApp.Domain.DTOs;

// Kullanıcı şifre değiştirme işlemi için kullanılan DTO (Data Transfer Object) sınıfı
public class UserChangePassword
{
    public string CurrentPassword { get; set; } = null!; // Eski şifre, zorunlu alan
    public string NewPassword { get; set; } = null!; // Yeni şifre, zorunlu alan
    public string ConfirmNewPassword { get; set; } = null!; // Yeni şifrenin doğrulaması, zorunlu alan
}
