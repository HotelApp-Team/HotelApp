using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces;

// IAuthRepository arayüzünün amacı, auth işlemleri için özel veri erişim sözleşmesini tanımlamaktır.
// Email kontrolü, email ile kullanıcı bulma, doğrulama tokenı ve rol sorgularını belirler.
// SOLID: ISP kullanır; sadece auth repository ihtiyaçlarını içerir.
// SOLID: DIP sağlar; servis katmanı somut repository yerine bu arayüze bağlı kalır.

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
