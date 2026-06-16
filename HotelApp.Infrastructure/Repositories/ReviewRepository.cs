using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApp.Infrastructure.Repositories
{
    /// <summary>
    /// Review entity'si icin veritabani erisim islemlerini yoneten repository sinifidir.
    /// Kullaniciya, otele, one cikan yorumlara ve admin sayfali listelemeye ait sorgulari gerceklestirir.
    /// </summary>
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        /// <summary>
        /// ReviewRepository sinifinin yeni bir ornegini olusturur.
        /// </summary>
        /// <param name="context">Veritabani islemleri icin kullanilan DbContext nesnesi.</param>
        public ReviewRepository(HotelAppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Belirtilen kullaniciya ait yorumlari kullanici ve otel bilgileriyle birlikte getirir.
        /// </summary>
        /// <param name="userId">Yorumlari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullaniciya ait yorum listesini doner.</returns>
        public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Belirtilen otele ait yorumlari kullanici ve otel bilgileriyle birlikte getirir.
        /// </summary>
        /// <param name="hotelId">Yorumlari listelenecek otelin Id degeri.</param>
        /// <returns>Otele ait yorum listesini doner.</returns>
        public async Task<IEnumerable<Review>> GetByHotelIdAsync(int hotelId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .Where(x => x.HotelId == hotelId && x.IsApproved)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Anasayfa vitrininde gosterilecek one cikan yorumlari puan ve tarihe gore siralayarak getirir.
        /// </summary>
        /// <param name="count">Getirilecek one cikan yorum sayisi.</param>
        /// <returns>One cikan yorum listesini doner.</returns>
        public async Task<IEnumerable<Review>> GetFeaturedAsync(int count)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
        }


        /// <summary>
        /// Belirtilen kullanici ve otele ait yorumu getirir.
        /// </summary>
        /// <param name="userId">Yorumu yazan kullanicinin Id degeri.</param>
        /// <param name="hotelId">Yorumun ait oldugu otelin Id degeri.</param>
        /// <returns>Yorum kaydi; bulunamazsa null.</returns>
        public async Task<Review?> GetByUserAndHotelAsync(int userId, int hotelId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.UserId == userId && x.HotelId == hotelId);
        }

        /// <summary>
        /// Verilen Id degerine sahip yorumu kullanici ve otel detaylariyla birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek yorumun Id degeri.</param>
        /// <returns>Yorum detayi; bulunamazsa null.</returns>
        public async Task<Review?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Admin yorum yonetim sayfasi icin tum yorumlari sayfali olarak getirir.
        /// Once toplam kayit sayisi alinir, ardindan ilgili sayfadaki kayitlar User ve Hotel bilgileriyle cekilir.
        /// </summary>
        /// <param name="page">Getirilecek sayfa numarasi.</param>
        /// <param name="pageSize">Sayfa basina getirilecek kayit sayisi.</param>
        /// <returns>Yorum listesi ve toplam kayit sayisini doner.</returns>
        public async Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedWithDetailsAsync(int page, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();

            var items = await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Admin onay islemi icin yorumu kullanici ve otel bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<Review?> GetForApprovalAsync(int id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Hotel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

    }
}
