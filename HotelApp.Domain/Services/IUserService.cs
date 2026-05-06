using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

// IUserService arayüzünün amacı, kullanıcı profil işlemleri için servis sözleşmesi tanımlamaktır.
// Profil getirme, profil güncelleme ve şifre değiştirme metotlarını belirler.
// SOLID: ISP kullanır; sadece kullanıcı profil servis ihtiyaçlarını içerir.
// SOLID: DIP sağlar; controller somut UserService yerine bu arayüze bağlı kalır.

public interface IUserService
{
    // Giris yapmis kullanicinin profil bilgilerini getirir
    Task<UserDto> GetProfileAsync(int userId);

    // Giris yapmis kullanicinin profil bilgilerini gunceller ve guncel halini dondurur
    Task<UserDto> UpdateProfileAsync(int userId, ProfileDto profileDto);

    // Giris yapmis kullanicinin sifresini degistirir
    Task ChangePasswordAsync(int userId, UserChangePasswordDto changePasswordDto);
}
