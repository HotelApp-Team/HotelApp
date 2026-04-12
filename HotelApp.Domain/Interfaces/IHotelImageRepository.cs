using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    public interface IHotelImageRepository
    {
        // Belirli bir otele ait tüm resimleri getir
        Task<IEnumerable<HotelImage>> GetByHotelIdAsync(int hotelId);

        // Tek bir resmi getir
        Task<HotelImage?> GetByIdAsync(int id);

        // Yeni resim ekle
        Task AddAsync(HotelImage image);

        // Resim güncelle
        void Update(HotelImage image);

        // Resim sil
        void Delete(HotelImage image);

        // Ana resmi getir (IsMain = true)
        Task<HotelImage?> GetMainImageAsync(int hotelId);

        // Değişiklikleri kaydet
        Task SaveChangesAsync();
    }
}
