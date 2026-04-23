using HotelApp.Domain.Entities;

// IAuthRepository, auth islemleri icin gerekli veri erisim sozlesmesini tanimlar.
// Amaci, auth tarafindaki ozel sorgulari standart repository'den ayirmaktir.
namespace HotelApp.Domain.Interfaces;

public interface IAuthRepository : IRepository<User>
{
    // Register öncesi email kontrolü
    Task<bool> EmailExistsAsync(string email);

    // Login için email'e göre kullanıcıyı getir
    Task<User?> GetByEmailAsync(string email);

    // Verify email işlemi için EmailVerificationToken alanına göre kullanıcıyı getirir
    Task<User?> GetByVerificationTokenAsync(string token);

    // Ada gore rolu getir (orn: "User", "Admin")
    Task<Role?> GetRoleByNameAsync(string roleName);
}
