using HotelApp.Domain.Entities;

namespace HotelApp.Domain.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        /// <summary>
        /// Belirtilen otele ait yorumlari getirir.
        /// </summary>
        /// <param name="hotelId">Yorumlari listelenecek otelin Id degeri.</param>
        /// <returns>Otele ait yorum listesini doner.</returns>
        Task<IEnumerable<Review>> GetByHotelIdAsync(int hotelId);

        /// <summary>
        /// Belirtilen kullaniciya ait yorumlari getirir.
        /// </summary>
        /// <param name="userId">Yorumlari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullaniciya ait yorum listesini doner.</returns>
        Task<IEnumerable<Review>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Anasayfada gosterilecek one cikan yorumlari getirir.
        /// </summary>
        /// <param name="count">Getirilecek one cikan yorum sayisi.</param>
        /// <returns>One cikan yorum listesini doner.</returns>
        Task<IEnumerable<Review>> GetFeaturedAsync(int count);

        /// <summary>
        /// Belirtilen kullanici ve otele ait yorumu getirir.
        /// </summary>
        /// <param name="userId">Yorumu yazan kullanicinin Id degeri.</param>
        /// <param name="hotelId">Yorumun ait oldugu otelin Id degeri.</param>
        /// <returns>Yorum kaydi; bulunamazsa null.</returns>
        Task<Review?> GetByUserAndHotelAsync(int userId, int hotelId);

        /// <summary>
        /// Verilen Id degerine sahip yorumu kullanici ve otel detaylariyla birlikte getirir.
        /// </summary>
        /// <param name="id">Getirilecek yorumun Id degeri.</param>
        /// <returns>Yorum detayi; bulunamazsa null.</returns>
        Task<Review?> GetWithDetailsAsync(int id);

        /// <summary>
        /// Admin paneli icin tum yorumlari sayfali olarak getirir.
        /// User ve Hotel navigation property'leri Include edilir.
        /// </summary>
        /// <param name="page">Getirilecek sayfa numarasi.</param>
        /// <param name="pageSize">Sayfa basina getirilecek kayit sayisi.</param>
        /// <returns>Yorum listesi ve toplam kayit sayisini doner.</returns>
        Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedWithDetailsAsync(int page, int pageSize);
    }
}
