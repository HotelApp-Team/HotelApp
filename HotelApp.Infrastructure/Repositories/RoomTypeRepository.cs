using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApp.Infrastructure.Repositories
{
    public class RoomTypeRepository : Repository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(HotelAppDbContext context) : base(context)
        {
        }

        public async Task<RoomType?> GetRoomTypeWithRoomsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.HotelRooms)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<RoomType>> GetAllWithRoomsAsync()
        {
            return await _dbSet
                .Include(x => x.HotelRooms)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomType>> GetByMaxGuestsAsync(int maxGuests)
        {
            return await _dbSet
                .Include(x => x.HotelRooms)
                .Where(x => x.MaxGuests >= maxGuests)
                .ToListAsync();
        }

        public async Task<RoomType?> GetByNameAsync(string name)
        {
            var normalizedName = name.Trim();
            return await _dbSet
                .Include(x => x.HotelRooms)
                .FirstOrDefaultAsync(x =>
                EF.Functions.Collate(x.Name, "Latin1_General_CI_AI") == normalizedName);
        }

    }
}
