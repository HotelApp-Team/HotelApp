using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //Değişiklikleri kayıt et
        Task<int> SaveChangesAsync();
    }
}
