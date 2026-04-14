using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
namespace HotelApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly HotelAppDbContext _context;
    public UserRepository(HotelAppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users
    .Include(u => u.Roles)
    .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
