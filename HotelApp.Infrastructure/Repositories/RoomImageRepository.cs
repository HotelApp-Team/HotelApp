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
    /// <summary>
    /// RoomImage entity'si icin veritabani erisim islemlerini yoneten repository sinifidir.
    /// Oda gorsellerini listeleme, ana gorsel getirme ve ana gorsel belirleme sorgularini gerceklestirir.
    /// </summary>
    public class RoomImageRepository : Repository<RoomImage>, IRoomImageRepository
    {
        private readonly HotelAppDbContext _context;

        /// <summary>
        /// RoomImageRepository sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="context">Veritabani islemleri icin kullanilan DbContext nesnesi.</param>
        public RoomImageRepository(HotelAppDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Belirtilen otel odasina ait gorselleri ana gorsel ve sira bilgisine gore siralayarak getirir.
        /// </summary>
        /// <param name="hotelRoomId">Gorselleri listelenecek otel odasinin Id degeri.</param>
        /// <returns>Odaya ait gorsel listesini doner.</returns>
        public async Task<List<RoomImage>> GetByHotelRoomIdAsync(int hotelRoomId)
        {
            return await _context.RoomImages
                .Where(x => x.HotelRoomId == hotelRoomId)
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Belirtilen otel odasina ait ana gorseli getirir.
        /// Ana gorsel yoksa sira bilgisine gore ilk gorsel doner.
        /// </summary>
        /// <param name="hotelRoomId">Ana gorseli getirilecek otel odasinin Id degeri.</param>
        /// <returns>Ana gorsel; bulunamazsa null.</returns>
        public async Task<RoomImage?> GetMainImageAsync(int hotelRoomId)
        {
            return await _context.RoomImages
                .Where(x => x.HotelRoomId == hotelRoomId)
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.DisplayOrder)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Belirtilen gorseli ilgili otel odasi icin ana gorsel olarak isaretler.
        /// Ayni odaya ait diger gorsellerin ana gorsel bilgisini false yapar.
        /// </summary>
        /// <param name="hotelRoomId">Ana gorseli guncellenecek otel odasinin Id degeri.</param>
        /// <param name="imageId">Ana gorsel olarak isaretlenecek gorselin Id degeri.</param>
        public async Task SetMainAsync(int hotelRoomId, int imageId)
        {
            var images = await _context.RoomImages
                .Where(x => x.HotelRoomId == hotelRoomId)
                .ToListAsync();

            foreach (var image in images)
                image.IsMain = image.Id == imageId;
        }
    }
}
