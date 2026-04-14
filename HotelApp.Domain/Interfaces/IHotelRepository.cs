using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    public interface IHotelRepository
    {
        // Tüm otelleri getir
        Task<IEnumerable<Hotel>> GetAllAsync();

        // Id’ye göre otel getir
        Task<Hotel?> GetByIdAsync(int id);

        // Detaylı otel (Include'lu)
        Task<Hotel?> GetWithDetailsAsync(int id);

        // Şehre göre filtreleme
        Task<IEnumerable<Hotel>> GetByCityAsync(string city);

        // Aktif oteller
        Task<IEnumerable<Hotel>> GetActiveAsync();

        // Yeni otel ekle
        Task AddAsync(Hotel hotel);

        // Güncelle
        void Update(Hotel hotel);

        // Sil 
        void Delete(Hotel hotel);
    }
}
