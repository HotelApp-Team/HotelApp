using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services;

public interface IUserService
{
    Task<UserDto> GetProfileAsync(int userId);

}
