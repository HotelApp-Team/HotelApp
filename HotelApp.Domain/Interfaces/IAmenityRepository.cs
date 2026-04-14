using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Interfaces
{
    public interface IAmenityRepository
    {
        // Tüm olanakları getir
        Task<List<Amenity>> GetAllAsync();

        // Id'ye göre getir
        Task<Amenity?> GetByIdAsync(int id);

        // Yeni olanak ekle
        Task AddAsync(Amenity amenity);

        // Güncelle
        Task UpdateAsync(Amenity amenity);

        // Sil
        Task DeleteAsync(int id);

        // Otele ait olanakları getir
        Task<List<Amenity>> GetAmenitiesByHotelIdAsync(int hotelId);

        // Otele olanak ekle
        Task AddAmenityToHotelAsync(int hotelId, int amenityId);

        // Otelden olanak kaldır
        Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId);
    }
}
