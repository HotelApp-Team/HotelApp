using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

public interface IUserService
{
    // Giris yapmis kullanicinin profil bilgilerini getirir
    Task<UserDto> GetProfileAsync(int userId);

    // Giris yapmis kullanicinin profil bilgilerini gunceller ve guncel halini dondurur
    Task<UserDto> UpdateProfileAsync(int userId, ProfileDto profileDto);

}
