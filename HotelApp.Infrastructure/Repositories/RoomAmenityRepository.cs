using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories;

public class RoomAmenityRepository : Repository<RoomAmenity>, IRoomAmenityRepository
{
    public RoomAmenityRepository(HotelAppDbContext context) : base(context)
    {
    }

    public async Task<RoomAmenity?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
    }

    public async Task<List<RoomAmenity>> GetByHotelRoomIdAsync(int hotelRoomId)
    {
        return await _context.HotelRooms
            .Where(x => x.Id == hotelRoomId)
            .SelectMany(x => x.RoomAmenities)
            .AsNoTracking()
            .ToListAsync();
    }
}
