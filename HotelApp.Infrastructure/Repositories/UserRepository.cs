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

    // Admin kullanıcı listesi için arama, aktiflik filtresi ve sayfalama uygular.
    public async Task<(List<User> Data, int TotalCount)> GetPagedUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize)
    {
        // Kullanıcı sorgusunu rollerle birlikte hazırlıyoruz.
        var query = _context.Users
            .Include(x => x.Roles)
            .AsNoTracking()
            .AsQueryable();

        // Search doluysa ad, soyad veya email içinde arama yapıyoruz.
        if (!string.IsNullOrEmpty(search))
        {
            var keywords = search.Trim();
            
            query = query.Where(x =>
                x.FirstName.Contains(keywords) ||
                x.LastName.Contains(keywords) ||
                x.Email.Contains(keywords));
        }

        // isActive geldiyse sadece aktif veya pasif kullanıcıları getiriyoruz.
        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        // Sayfalama cevabı için toplam kayıt sayısını alıyoruz.
        var totalCount = await query.CountAsync();

        // İstenen sayfadaki kullanıcıları en yeni kayıt önce olacak şekilde çekiyoruz.
        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }
}
