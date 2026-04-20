using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

// UserRepository, kullanici verilerini ve profile ozel sorgulari veritabanindan getirir.
// Amaci, user tarafindaki data access kodunu service katmanindan ayirmaktir.
// SOLID: SRP kullanir, OCP icin Repository<User> yapisini genisletir.
namespace HotelApp.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    // Constructor ile DbContext'i base repository'ye gonderiyoruz
    public UserRepository(HotelAppDbContext context) : base(context)
    {
    }

    public override async Task<User?> GetByIdAsync(int userId)
    {
        // 1. Kullaniciyi ID'ye gore getir
        // 2. Rollerini de Include ile birlikte yukle
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
