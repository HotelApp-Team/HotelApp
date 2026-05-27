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
using System.Security.Cryptography.X509Certificates;


namespace HotelApp.Infrastructure.Repositories
{
    /// <summary>
    /// HotelRoom entity'si icin veritabani erisim islemlerini yoneten repository sinifidir.
    /// Oda detaylari, otele gore odalar, oda tipine gore odalar ve musait oda arama sorgularini gerceklestirir.
    /// </summary>
    public class HotelRoomRepository : Repository<HotelRoom>, IHotelRoomRepository
    {
        /// <summary>
        /// HotelRoomRepository sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="context">Veritabani islemleri icin kullanilan DbContext nesnesi.</param>
        public HotelRoomRepository(HotelAppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Verilen Id degerine sahip otel odasini otel, oda tipi, rezervasyon ve imkan detaylariyla birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek otel odasinin Id degeri.</param>
        /// <returns>Detaylariyla birlikte otel odasi; bulunamazsa null.</returns>
        public async Task<HotelRoom?> GetRoomWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .Include(x => x.RoomAmenities)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Tum otel odalarini otel, oda tipi, rezervasyon ve imkan detaylariyla birlikte getirir.
        /// </summary>
        /// <returns>Detaylariyla birlikte otel odasi listesini doner.</returns>
        public async Task<IEnumerable<HotelRoom>> GetAllWithRoomsAsync()
        {
            return await _dbSet
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .Include(x => x.RoomAmenities)
                .ToListAsync();
        }

        /// <summary>
        /// Belirtilen misafir sayisini karsilayabilecek kapasitedeki otel odalarini getirir.
        /// </summary>
        /// <param name="maxGuests">Gerekli minimum misafir kapasitesi.</param>
        /// <returns>Kapasiteye uygun otel odasi listesini doner.</returns>
        public async Task<IEnumerable<HotelRoom>> GetByMaxGuestsAsync(int maxGuests)
        {
            return await _dbSet
                .Include(x => x.RoomType)
                .Include(x => x.Hotel)
                .Include(x => x.RoomAmenities)
                .Where(x => x.RoomType.MaxGuests >= maxGuests)
                .ToListAsync();
        }

        /// <summary>
        /// Oda tipi adina gore otel odalarini getirir.
        /// </summary>
        /// <param name="name">Aranacak oda tipi adi veya ad parcasi.</param>
        /// <returns>Oda tipi adina uyan otel odasi listesini doner.</returns>
        public async Task<IEnumerable<HotelRoom>> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(x => x.RoomType)
                .Include(x => x.Hotel)
                .Include(x => x.RoomAmenities)
                .Where(x => x.RoomType.Name.Contains(name))
                .ToListAsync();
        }

        /// <summary>
        /// Tum otel odalarini iliskili detay verileriyle birlikte getirir.
        /// </summary>
        /// <returns>Detaylariyla birlikte otel odasi listesini doner.</returns>
        public async Task<IEnumerable<HotelRoom>> GetRoomsWithDetailsAsync()
        {
            return await _dbSet
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .Include(x => x.RoomAmenities)
                .ToListAsync();
        }

        /// <summary>
        /// Belirtilen otele ait odalari otel, oda tipi, rezervasyon, imkan ve gorsel bilgileriyle birlikte getirir.
        /// </summary>
        /// <param name="hotelId">Odalari listelenecek otelin Id degeri.</param>
        /// <returns>Belirtilen otele ait otel odasi listesini doner.</returns>
        public async Task<IEnumerable<HotelRoom>> GetRoomsByHotelAsync(int hotelId)
        {
            return await _dbSet
                .Where(x => x.HotelId == hotelId)
                .Include(x => x.Hotel)
                .Include(x => x.RoomType)
                .Include(x => x.Reservations)
                .Include(x => x.RoomAmenities)
                .Include(x => x.RoomImages)
                .ToListAsync();
        }

        /// <summary>
        /// Belirtilen oda tipine sahip otel odalarini getirir.
        /// </summary>
        /// <param name="roomTypeId">Filtreleme icin kullanilacak oda tipi Id degeri.</param>
        /// <returns>Belirtilen oda tipine ait otel odasi listesini doner.</returns>
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

        /// <summary>
        /// Stored procedure kullanarak belirtilen otel, sehir, tarih araligi ve misafir sayisina gore musait odalari getirir.
        /// Sonuc kayitlarina oda imkanlari ve oda gorselleri manuel olarak eklenir.
        /// </summary>
        /// <param name="hotelId">Arama yapilacak otelin Id degeri.</param>
        /// <param name="city">Otelin bulundugu sehir.</param>
        /// <param name="checkIn">Giris tarihi.</param>
        /// <param name="checkOut">Cikis tarihi.</param>
        /// <param name="guestCount">Misafir sayisi.</param>
        /// <returns>Musait oda DTO listesini doner.</returns>
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

            var roomIds = result
                .Select(x => x.HotelRoomId)
                .Distinct()
                .ToList();

            if (!roomIds.Any())
            {
                return result;
            }

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

            var roomImages = await _context.RoomImages
                .Where(x => roomIds.Contains(x.HotelRoomId))
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.DisplayOrder)
                .AsNoTracking()
                .ToListAsync();

            foreach (var room in result)
            {
                room.RoomAmenities = amenitiesByRoom.TryGetValue(room.HotelRoomId, out var amenities)
                    ? amenities
                    : new List<RoomAmenityDto>();

                var images = roomImages
                    .Where(x => x.HotelRoomId == room.HotelRoomId)
                    .OrderByDescending(x => x.IsMain)
                    .ThenBy(x => x.DisplayOrder)
                    .ToList();

                room.MainImageUrl = images
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault();

                room.Images = images.Select(x => new RoomImageDto
                {
                    Id = x.Id,
                    HotelRoomId = x.HotelRoomId,
                    ImageUrl = x.ImageUrl,
                    IsMain = x.IsMain,
                    DisplayOrder = x.DisplayOrder
                }).ToList();
            }

            return result;
        }
    }
}