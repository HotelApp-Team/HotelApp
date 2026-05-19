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
                .Include(x => x.RoomAmenities)
                .FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<IEnumerable<HotelRoom>> GetAllWithRoomsAsync()
        {
            return await _dbSet
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .Include(x => x.RoomAmenities)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetByMaxGuestsAsync(int maxGuests)
        {
            return await _dbSet
                .Include(x => x.RoomType)
                .Include(x => x.Hotel)
                .Include(x => x.RoomAmenities)
                .Where(x => x.RoomType.MaxGuests >= maxGuests)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(x => x.RoomType)
                .Include(x => x.Hotel)
                .Include(x => x.RoomAmenities)
                .Where(x => x.RoomType.Name.Contains(name))
                .ToListAsync();
        }


        public async Task<IEnumerable<HotelRoom>> GetRoomsWithDetailsAsync()
        {
            return await _dbSet
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .Include(x => x.RoomAmenities)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetRoomsByHotelAsync(int hotelId)
        {
            return await _dbSet
                .Where(x => x.HotelId == hotelId)
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .Include(x => x.RoomAmenities)
                .ToListAsync();
        }

        public async Task<IEnumerable<HotelRoom>> GetRoomsByRoomTypeAsync(int roomTypeId)
        {
            return await _dbSet
                .Where(x => x.RoomTypeId == roomTypeId)
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .Include(x => x.RoomAmenities)
                .ToListAsync();
        }

        public async Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(int hotelId, string city, DateTime checkIn, DateTime checkOut, int guestCount)
        {
            var result = await _context.Database
                .SqlQueryRaw<AvailableRoomDto>(
                "EXEC sp_SearchAvailableRooms @HotelId = @HotelId, @City = @City, @CheckInDate = @CheckInDate, @CheckOutDate = @CheckOutDate, @GuestCount = @GuestCount",
                new SqlParameter("@HotelId", hotelId),
                new SqlParameter("@City", city),
                new SqlParameter("@CheckInDate", checkIn.Date),
                new SqlParameter("@CheckOutDate", checkOut.Date),
                new SqlParameter("@GuestCount", guestCount))
                .ToListAsync();

            var roomIds = result.Select(x => x.HotelRoomId).Distinct().ToList();

            if (roomIds.Any())
            {
                var rooms = await _context.HotelRooms
                    .Where(x => roomIds.Contains(x.Id))
                    .Include(x => x.RoomAmenities)
                    .AsNoTracking()
                    .ToListAsync();

                var amenitiesByRoom = rooms.ToDictionary(
                    x => x.Id,
                    x => x.RoomAmenities
                        .Select(a => new RoomAmenityDto
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Icon = a.Icon
                        })
                        .ToList());

                foreach (var room in result)
                {
                    room.RoomAmenities = amenitiesByRoom.TryGetValue(room.HotelRoomId, out var amenities)
                        ? amenities
                        : new List<RoomAmenityDto>();
                }
            }

            return result;
        }

    }
}
