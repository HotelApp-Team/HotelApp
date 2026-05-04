using HotelApp.Domain.DTOs;

// IUserService, kullanici profil islemleri icin servis sozlesmesini tanimlar.
// Amaci, controller'in somut UserService'e bagimli kalmamasidir.
// SOLID: DIP ve ISP kullanir.
namespace HotelApp.Domain.Services;

public interface IUserService
{
    // Giris yapmis kullanicinin profil bilgilerini getirir
    Task<UserDto> GetProfileAsync(int userId);

    // Giris yapmis kullanicinin profil bilgilerini gunceller ve guncel halini dondurur
    Task<UserDto> UpdateProfileAsync(int userId, ProfileDto profileDto);

    // Giris yapmis kullanicinin sifresini degistirir
    Task ChangePasswordAsync(int userId, UserChangePassword changePasswordDto);
}
