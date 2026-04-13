using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);

}
