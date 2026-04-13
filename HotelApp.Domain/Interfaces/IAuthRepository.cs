using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces
{
    public interface IAuthRepository
    {
        //Register öncesi email kontrolü
        Task<bool> EmailExistsAsync(string email);

        //Login için; Email'e göre kullanıcıyı getir. Passwordhash dahil
        Task<User?> GetByEmailAsync(string email);

        //Yeni kullanıcı ekle (Register sonrası)
        Task<User> AddAsync(User user);
    }
}
