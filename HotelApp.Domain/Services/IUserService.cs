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

    // Admin paneli için kullanıcıları filtreli ve sayfalı şekilde getirir.
    Task<PagedResponseDto<UserDto>> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize);

}
