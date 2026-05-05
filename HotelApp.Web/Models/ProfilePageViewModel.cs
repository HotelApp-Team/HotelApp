namespace HotelApp.Web.Models;

// ProfilePageViewModel sınıfının amacı, profil sayfasındaki formları tek model altında toplamaktır.
// Profil güncelleme ve şifre değiştirme alanlarını aynı View içinde kullanılabilir hale getirir.
// SOLID: SRP kullanır; sadece profil sayfasının ekran verisini taşır.
public class ProfilePageViewModel
{
    // Profil bilgileri formunda gösterilecek kullanıcı verisini taşır.
    public ProfileViewModel Profile { get; set; } = null!;
    // Şifre değiştirme formunda kullanılacak alanları taşır.
    public ChangePasswordViewModel ChangePassword { get; set; } = null!;
}
