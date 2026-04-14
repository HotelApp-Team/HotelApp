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
    public class HotelRoomRepository : IHotelRoomRepository
    {
        private readonly HotelAppDbContext _context;
        public HotelRoomRepository(HotelAppDbContext context) => _context = context;

        public async Task<HotelRoom?> GetByIdAsync(int id)
        {
            return await _context.HotelRooms.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<HotelRoom>> GetAllAsync()
        {
            return await _context.HotelRooms.ToListAsync();
        }

        public async Task<HotelRoom?> GetRoomWithDetailsAsync(int id)
        {
            return await _context.HotelRooms
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<IEnumerable<HotelRoom>> GetAllWithRoomsAsync()
        {
            return await _context.HotelRooms
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetByMaxGuestsAsync(int maxGuests)
        {
            return await _context.HotelRooms
                .Include(x => x.RoomType)
                .Include(x => x.Hotel)
                .Where(x => x.RoomType.MaxGuests >= maxGuests)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetByNameAsync(string name)
        {
            return await _context.HotelRooms
                .Include(x => x.RoomType)
                .Include(x => x.Hotel)
                .Where(x => x.RoomType.Name.Contains(name))
                .ToListAsync();
        }

        public async Task AddAsync(HotelRoom entity)
        {
            await _context.HotelRooms.AddAsync(entity);
        }

        public async Task UpdateAsync(HotelRoom entity)
        {
            _context.HotelRooms.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.HotelRooms.Remove(entity);
            }
        }



        public async Task<IEnumerable<HotelRoom>> GetRoomsWithDetailsAsync()
        {
            return await _context.HotelRooms
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetRoomsByHotelAsync(int hotelId)
        {
            return await _context.HotelRooms
                .Where(x => x.HotelId == hotelId)
                .Include(x => x.RoomType)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetRoomsByRoomTypeAsync(int roomTypeId)
        {
            return await _context.HotelRooms
                .Where(x => x.RoomTypeId == roomTypeId)
                .Include(x => x.Hotel)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            var checkInDateOnly = DateOnly.FromDateTime(checkIn);
            var checkOutDateOnly = DateOnly.FromDateTime(checkOut);

            return await _context.HotelRooms
                .Include(x => x.Reservations)
                .Where(x => !x.Reservations.Any(r => r.CheckInDate < checkOutDateOnly &&
                    r.CheckOutDate > checkInDateOnly))
                .ToListAsync();
        }
    }
}
