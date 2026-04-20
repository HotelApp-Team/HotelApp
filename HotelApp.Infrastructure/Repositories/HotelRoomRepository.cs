using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing.CompositionExtensions;

namespace HotelApp.Infrastructure.Repositories
{
    public class HotelRoomRepository : Repository<HotelRoom>, IHotelRoomRepository
    {
        public HotelRoomRepository(HotelAppDbContext context) : base(context)
        {
        }

        public async Task<HotelRoom?> GetRoomWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<IEnumerable<HotelRoom>> GetAllWithRoomsAsync()
        {
            return await _dbSet
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetByMaxGuestsAsync(int maxGuests)
        {
            return await _dbSet
                .Include(x => x.RoomType)
                .Include(x => x.Hotel)
                .Where(x => x.RoomType.MaxGuests >= maxGuests)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(x => x.RoomType)
                .Include(x => x.Hotel)
                .Where(x => x.RoomType.Name.Contains(name))
                .ToListAsync();
        }


        public async Task<IEnumerable<HotelRoom>> GetRoomsWithDetailsAsync()
        {
            return await _dbSet
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetRoomsByHotelAsync(int hotelId)
        {
            return await _dbSet
                .Where(x => x.HotelId == hotelId)
                .Include(x => x.RoomType)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetRoomsByRoomTypeAsync(int roomTypeId)
        {
            return await _dbSet
                .Where(x => x.RoomTypeId == roomTypeId)
                .Include(x => x.Hotel)
                .ToListAsync();
        }

        public async Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            var checkInDateOnly = new SqlParameter("@CheckIn", checkIn);
            var checkOutDateOnly = new SqlParameter("@CheckOut", checkOut);

            var result = await _context.Set<AvailableRoomDto>()
                .FromSqlRaw("EXEC SP_GetAvailableRooms @CheckIn, @CheckOut", checkInDateOnly, checkOutDateOnly)
                .ToListAsync();
            return result;
        }

    }
}
