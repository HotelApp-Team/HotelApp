using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

// UserRepository sınıfının amacı, kullanıcı profil işlemleri için gerekli verileri getirmektir.
// Kullanıcıyı rollerle birlikte veritabanından okur ve service katmanına hazır veri sağlar.
// SOLID: SRP kullanır; sadece kullanıcı veri erişiminden sorumludur.
// SOLID: OCP kullanır; Repository<User> yapısını kullanıcıya özel sorguyla genişletir.

public class UserRepository : Repository<User>, IUserRepository
{
    // Constructor ile DbContext'i base repository'ye gonderiyoruz.
    public UserRepository(HotelAppDbContext context) : base(context)
    {
    }

    public override async Task<User?> GetByIdAsync(int userId)
    {
        // 1. Kullaniciyi ID'ye gore getir.
        // 2. Rollerini de Include ile birlikte yukle.
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
