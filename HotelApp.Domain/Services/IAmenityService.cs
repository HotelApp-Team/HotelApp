using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    // Otel olanaklarıyla ilgili yapılabilecek işlemleri tanımlar.
    // Controller katmanı bu interface üzerinden olanak listeleme, ekleme,
    // güncelleme, silme ve otele olanak atama işlemlerini çağırır.
    public interface IAmenityService
    {
        // Sistemde kayıtlı tüm olanakları getirir.
        // Admin panelinde olanak listesini göstermek için kullanılır.
        Task<List<AmenityDto>> GetAllAsync();

        // Id değerine göre tek bir olanak bilgisini getirir.
        // Düzenleme veya detay işlemlerinde ilgili kaydı bulmak için kullanılır.
        Task<AmenityDto?> GetByIdAsync(int id);

        // Yeni olanak kaydı oluşturur.
        // Örneğin Wi-Fi, otopark, havuz gibi otel özellikleri bu metotla eklenir.
        Task AddAsync(AmenityCreateDto dto);

        // Mevcut olanak kaydını günceller.
        // Olanak adı veya ikon bilgisi değiştirildiğinde kullanılır.
        Task UpdateAsync(AmenityUpdateDto dto);

        // Id değerine göre olanak kaydını siler.
        // Kullanılan olanakların silinmemesi servis katmanında kontrol edilir.
        Task DeleteAsync(int id);

        // Belirtilen otele atanmış olanakları getirir.
        // Otel detay sayfasında veya admin otel düzenleme ekranında kullanılır.
        Task<List<AmenityDto>> GetAmenitiesByHotelIdAsync(int hotelId);

        // Bir otele birden fazla olanak atar.
        // Liste içindeki geçerli olanaklar eklenir, bulunamayan Id değerleri response içinde döndürülür.
        Task<HotelAmenityAssignResponseDto> AddAmenitiesToHotelAsync(int hotelId, List<int> amenityIds);

        // Belirtilen otelden seçilen olanağı kaldırır.
        // Otel ile olanak arasındaki ilişkiyi silmek için kullanılır.
        Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId);
    }
}
