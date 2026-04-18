using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

// AuthRepository, login ve email verify gibi auth sorgularini veritabaninda calistirir.
// Amaci, auth'a ozel data access kodunu tek yerde toplamaktir.
// SOLID: SRP kullanir, OCP icin Repository<User> yapisini genisletir.
namespace HotelApp.Infrastructure.Repositories;

public class AuthRepository : Repository<User>, IAuthRepository
{
    // Constructor ile DbContext'i base repository'ye gönderiyoruz
    public AuthRepository(HotelAppDbContext context) : base(context)
    {
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByVerificationTokenAsync(string token)
    {
        // 1. Kullanıcıyı EmailVerificationToken alanına göre bul
        // 2. JWT üretiminde kullanılacağı için rollerini de Include ile birlikte yükle
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
    }
}
