using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using HotelApp.Infrastructure.Context;

namespace HotelApp.Infrastructure.Repositories;
public class AuthRepository : IAuthRepository
{
    private readonly HotelAppDbContext _context;
    public AuthRepository(HotelAppDbContext context)
    {
        _context = context;  //Program.cs'de DI ile eklemiştik, burada da constructor ile alıyoruz
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);  //Kayit var mı kontrolü, varsa true döner
    }

    public async Task<User?> GetByEmailAsync(string email) //Kullanıcıyı email ile getirir, token oluşturmak için gerekli. Include almazsa bos döner.
    {
        return await _context.Users
            .Include(u => u.Roles) // Token'de roller lazım
            .FirstOrDefaultAsync(u => u.Email == email); //Email'e göre kullanıcıyı getirir, yoksa null döner
    }

    public async Task<User> AddAsync(User user) //Yeni kullanıcı ekler, token oluşturmak için gerekli
    {
        await _context.Users.AddAsync(user);
        return user;
    }
}
