using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApp.Infrastructure.Repositories
{
    /// <summary>
    /// RoomType entity'si icin veritabani erisim islemlerini yoneten repository sinifidir.
    /// Oda tiplerini iliskili otel odalariyla birlikte getirme, kapasiteye gore filtreleme
    /// ve ada gore arama sorgularini gerceklestirir.
    /// </summary>
    public class RoomTypeRepository : Repository<RoomType>, IRoomTypeRepository
    {
        /// <summary>
        /// RoomTypeRepository sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="context">Veritabani islemleri icin kullanilan DbContext nesnesi.</param>
        public RoomTypeRepository(HotelAppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Verilen Id degerine sahip oda tipini iliskili otel odalariyla birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek oda tipinin Id degeri.</param>
        /// <returns>Oda tipi ve iliskili otel odalari; bulunamazsa null.</returns>
        public async Task<RoomType?> GetRoomTypeWithRoomsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.HotelRooms)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Tum oda tiplerini iliskili otel odalariyla birlikte getirir.
        /// </summary>
        /// <returns>Otel odalariyla birlikte oda tipi listesini doner.</returns>
        public async Task<IEnumerable<RoomType>> GetAllWithRoomsAsync()
        {
            return await _dbSet
                .Include(x => x.HotelRooms)
                .ToListAsync();
        }

        /// <summary>
        /// Belirtilen misafir sayisini karsilayabilecek oda tiplerini getirir.
        /// </summary>
        /// <param name="maxGuests">Gerekli minimum misafir kapasitesi.</param>
        /// <returns>Kapasiteye uygun oda tipi listesini doner.</returns>
        public async Task<IEnumerable<RoomType>> GetByMaxGuestsAsync(int maxGuests)
        {
            return await _dbSet
                .Include(x => x.HotelRooms)
                .Where(x => x.MaxGuests >= maxGuests)
                .ToListAsync();
        }

        /// <summary>
        /// Verilen ada sahip oda tipini getirir.
        /// Arama, Latin1_General_CI_AI collation ile buyuk/kucuk harf ve aksan duyarsiz yapilir.
        /// </summary>
        /// <param name="name">Aranacak oda tipi adi.</param>
        /// <returns>Oda tipi; bulunamazsa null.</returns>
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