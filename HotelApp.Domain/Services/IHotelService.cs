using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HotelApp.Domain.Services
{
    // Oteller icin listeleme, detay getirme, arama,
    // ekleme, guncelleme, silme ve admin filtreleme islemlerini tanimlar.
    public interface IHotelService
    {
        // Tum otelleri liste halinde getirir.
        // Otel liste DTO listesini doner
        Task<IEnumerable<HotelListDto>> GetAllAsync();

        // Belirtilen Id degerine sahip otelin detaylarini getirir.
        // Otel bulunamazsa null doner.
        Task<HotelDetailDto?> GetByIdAsync(int id);

        // Belirtilen sehre ait otelleri getirir.
        // Sehir filtresine uygun otel DTO listesini doner.
        Task<IEnumerable<HotelListDto>> GetByCityAsync(string city);

        // Sadece aktif durumdaki otelleri getirir.
        // Aktif otel DTO listesini doner.
        Task<IEnumerable<HotelListDto>> GetActiveAsync();

        // Yeni otel kaydi olusturur.
        // Olusturulan otelin Id degerini doner.
        Task<int> CreateAsync(HotelCreateDto dto);

        // Mevcut otel kaydini gunceller.
        // Otel bilgilerini DTO'dan gelen degerlerle yeniler.
        Task UpdateAsync(HotelUpdateDto dto);

        // Belirtilen Id degerine sahip oteli pasif duruma alir.
        // Kaydi tamamen silmek yerine IsActive alanini false yapar.
        Task DeleteAsync(int id);

        // Sistemde aktif oteli bulunan sehirleri getirir.
        // Sehir adlari listesini doner.
        Task<List<string>> GetCitiesAsync();

        // Ana sayfa hero alaninda gosterilecek otel ve sehir istatistiklerini getirir.
        // Toplam aktif otel ve sehir sayisini doner.
        Task<HeroStatsDto> GetHeroStatsAsync();

        // Ana sayfada gosterilecek one cikan sehir kartlarini getirir.
        // Istenen sayida sehir karti DTO listesi doner.
        Task<List<CityCardDto>> GetFeaturedCitiesAsync(int count);

        // Kullanici arama kriterlerine gore otel aramasi yapar.
        // Tarih, sehir, misafir sayisi ve filtrelere uygun otelleri doner.
        Task<HotelSearchResponseDto> SearchAsync(HotelSearchRequestDto dto, int page, int pageSize);

        // Admin otel listesi icin filtreleme, siralama ve sayfalama yapar.
        // Sayfalanmis otel liste cevabini doner.
        Task<PagedResponseDto<HotelListDto>> GetAllFilteredAsync(string? city, byte? star, bool? isActive, string? sortBy, string? sortOrder, int page, int pageSize);
    }
}
