using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    /// <summary>
    /// Otel yorumlari icin listeleme, one cikan yorumlari getirme,
    /// yorum olusturma, silme ve admin sayfali listeleme islemlerini tanimlar.
    /// </summary>
    public interface IReviewService
    {
        /// <summary>
        /// Belirtilen otele ait yorumlari getirir.
        /// </summary>
        /// <param name="hotelId">Yorumlari listelenecek otelin Id degeri.</param>
        /// <returns>Otele ait yorum DTO listesini doner.</returns>
        Task<IEnumerable<ReviewDto>> GetHotelReviewAsync(int hotelId);

        /// <summary>
        /// Belirtilen kullaniciya ait yorumlari getirir.
        /// </summary>
        /// <param name="userId">Yorumlari listelenecek kullanicinin Id degeri.</param>
        /// <returns>Kullaniciya ait yorum DTO listesini doner.</returns>
        Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId);

        /// <summary>
        /// Anasayfada gosterilecek one cikan yorumlari getirir.
        /// </summary>
        /// <param name="count">Getirilecek one cikan yorum sayisi.</param>
        /// <returns>One cikan yorum DTO listesini doner.</returns>
        Task<IEnumerable<ReviewDto>> GetFeaturedReviewsAsync(int count);

        /// <summary>
        /// Belirtilen kullanici icin yeni yorum olusturur.
        /// </summary>
        /// <param name="userId">Yorumu olusturan kullanicinin Id degeri.</param>
        /// <param name="dto">Yorum olusturma bilgilerini iceren DTO.</param>
        /// <returns>Olusturulan yorum DTO nesnesini doner.</returns>
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto);

        /// <summary>
        /// Belirtilen yorumu siler.
        /// Kullanici yalnizca kendi yorumunu silebilir; admin baskasina ait yorumlari da silebilir.
        /// </summary>
        /// <param name="id">Silinecek yorumun Id degeri.</param>
        /// <param name="userId">Islemi yapan kullanicinin Id degeri.</param>
        /// <param name="isAdmin">Islemi yapan kullanicinin admin olup olmadigini belirtir.</param>
        Task DeleteReviewAsync(int id, int userId, bool isAdmin);

        /// <summary>
        /// Admin yorum yonetim ekrani icin tum yorumlari sayfali olarak getirir.
        /// </summary>
        /// <param name="page">Getirilecek sayfa numarasi.</param>
        /// <param name="pageSize">Sayfa basina getirilecek kayit sayisi.</param>
        /// <returns>Sayfalanmis yorum DTO sonucunu doner.</returns>
        Task<PagedResponseDto<ReviewDto>> GetAllReviewsAsync(int page, int pageSize);

        /// <summary>
        /// Admin tarafindan belirtilen yorumu yayina alir.
        /// </summary>
        /// <param name="id">Onaylanacak yorumun Id degeri.</param>
        /// <returns>Onaylanan yorum DTO nesnesini doner.</returns>
        Task<ReviewDto> ApproveReviewAsync(int id);
    }
}
