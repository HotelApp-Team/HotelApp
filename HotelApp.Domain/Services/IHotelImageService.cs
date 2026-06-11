using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    // Otel görselleriyle ilgili yapılabilecek işlemleri tanımlar.
    // Bu interface sayesinde controller, görsel işlemlerini doğrudan repository'ye gitmeden servis üzerinden çağırır.
    // Böylece otel görseli ekleme, silme, güncelleme ve ana görsel seçme işlemleri tek bir servis sözleşmesinde toplanır.
    public interface IHotelImageService
    {
        // Verilen hotelId değerine ait tüm görselleri getirir.
        // Admin panelinde veya otel detay sayfasında otelin görsel galerisini listelemek için kullanılır.
        Task<List<HotelImageDto>> GetByHotelIdAsync(int hotelId);

        // Verilen id değerine sahip tek bir görsel kaydını getirir.
        // Güncelleme veya silme işleminden önce ilgili görselin var olup olmadığını kontrol etmek için kullanılabilir.
        Task<HotelImageDto?> GetByIdAsync(int id);

        // Belirtilen otelin ana görsel olarak işaretlenmiş görselini getirir.
        // Otel kartlarında veya liste ekranlarında gösterilecek kapak görselini bulmak için kullanılır.
        Task<HotelImageDto?> GetMainImageAsync(int hotelId);

        // Belirtilen otele yeni bir görsel ekler.
        // Eğer eklenen görsel ana görsel olarak seçildiyse, servis diğer görsellerin ana görsel durumunu düzenler.
        Task AddAsync(int hotelId, HotelImageCreateDto dto);

        // Belirtilen otele ait mevcut bir görselin bilgilerini günceller.
        // Görsel URL'si, ana görsel durumu ve ekranda gösterilme sırası bu işlemle değiştirilebilir.
        Task UpdateAsync(int hotelId, HotelImageUpdateDto dto);

        // Belirtilen otele ait bir görseli siler.
        // hotelId kontrolü ile başka bir otele ait görselin yanlışlıkla silinmesi engellenir.
        Task DeleteAsync(int hotelId, int imageId);

        // Belirtilen görseli otelin ana görseli olarak ayarlar.
        // Böylece otel listeleme ve detay ekranlarında hangi görselin öne çıkacağı belirlenir.
        Task SetMainImageAsync(int hotelId, int imageId);
    }
}
