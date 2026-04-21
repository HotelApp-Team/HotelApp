using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


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
                .Include(x=>x.Hotel)
                .Include(x => x.RoomType)
                .Include(x=>x.Reservations)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetRoomsByRoomTypeAsync(int roomTypeId)
        {
            return await _dbSet
                .Where(x => x.RoomTypeId == roomTypeId)
                .Include(x => x.Hotel)
                .Include(x=>x.RoomType)
                .Include(x=>x.Reservations)
                .ToListAsync();
        }

        public async Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(string city, DateTime checkIn, DateTime checkOut, int guestCount)
        {
            var result = await _context.Database
                .SqlQueryRaw<AvailableRoomDto>(
                "EXEC sp_SearchAvailableRooms @City, @CheckInDate, @CheckOutDate, @GuestCount",
                new SqlParameter("@City", city),
                new SqlParameter("@CheckInDate", checkIn.Date),
                new SqlParameter("@CheckOutDate", checkOut.Date),
                new SqlParameter("@GuestCount", guestCount))
                .ToListAsync();

            return result;
        }

    }
}
