using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

public interface IAuthRepository : IRepository<User>
{
    // Register öncesi email kontrolü
    Task<bool> EmailExistsAsync(string email);

    // Login için email'e göre kullanıcıyı getir
    Task<User?> GetByEmailAsync(string email);
      
    // Role ekle
    Task<Role?> GetRoleByNameAsync(string roleName);
}
          