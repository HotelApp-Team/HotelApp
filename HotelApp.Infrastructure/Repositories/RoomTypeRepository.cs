using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Infrastructure.Repositories
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly HotelAppDbContext _context;
        public RoomTypeRepository(HotelAppDbContext context) => _context = context;

        public async Task<RoomType?> GetByIdAsync(int id)
        {
            return await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<RoomType>> GetAllAsync()
        {
            return await _context.RoomTypes.ToListAsync();
        }

        public async Task<RoomType?> GetRoomTypeWithRoomsAsync(int id)
        {
            return await _context.RoomTypes
                .Include(x => x.HotelRooms)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<RoomType>> GetAllWithRoomsAsync()
        {
            return await _context.RoomTypes
                .Include(x => x.HotelRooms)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomType>> GetByMaxGuestsAsync(int maxGuests)
        {
            return await _context.RoomTypes
                .Include(x => x.HotelRooms)
                .Where(x => x.MaxGuests >= maxGuests)
                .ToListAsync();
        }

        public async Task<RoomType?> GetByNameAsync(string name)
        {
            return await _context.RoomTypes
                .Include(x => x.HotelRooms)
                .FirstOrDefaultAsync(x => x.Name.Contains(name));
        }

        public async Task AddAsync(RoomType entity)
        {
            await _context.RoomTypes.AddAsync(entity);
        }

        public async Task UpdateAsync(RoomType entity)
        {
            _context.RoomTypes.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.RoomTypes.Remove(entity);
            }
        }

        public async Task AddRoomTypeAsync(RoomType entity)
        {
            await AddAsync(entity);
        }
    }
}
