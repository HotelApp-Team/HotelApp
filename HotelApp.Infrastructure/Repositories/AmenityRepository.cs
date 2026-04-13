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
    public class AmenityRepository : IAmenityRepository
    {
      private readonly HotelAppDbContext _context;

        public AmenityRepository(HotelAppDbContext context)
        {
            _context = context;
        }

        // Tüm olanakları getir
        public async Task<List<Amenity>> GetAllAsync()
        {
            return await _context.Amenities
                .AsNoTracking()
                .ToListAsync();
        }

        // Id'ye göre getir
        public async Task<Amenity?> GetByIdAsync(int id)
        {
            return await _context.Amenities
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Yeni ekle
        public async Task AddAsync(Amenity amenity)
        {
            await _context.Amenities.AddAsync(amenity);
        }

        // Güncelle
        public Task UpdateAsync(Amenity amenity)
        {
            _context.Amenities.Update(amenity);
            return Task.CompletedTask;
        }

        // Sil
        public async Task DeleteAsync(int id)
        {
            var amenity = await _context.Amenities.FindAsync(id);
            if (amenity != null)
            {
                _context.Amenities.Remove(amenity);
            }
        }

        // Otele ait olanakları getir
        public async Task<List<Amenity>> GetAmenitiesByHotelIdAsync(int hotelId)
        {
            return await _context.Hotels
                .Where(h => h.Id == hotelId)
                .SelectMany(h => h.Amenities)
                .AsNoTracking()
                .ToListAsync();
        }

        // Otele olanak ekle (many-to-many)
        public async Task AddAmenityToHotelAsync(int hotelId, int amenityId)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Amenities)
                .FirstOrDefaultAsync(h => h.Id == hotelId);

            var amenity = await _context.Amenities.FindAsync(amenityId);

            if (hotel != null && amenity != null)
            {
                hotel.Amenities.Add(amenity);
            }
        }

        // Otelden olanak kaldır
        public async Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Amenities)
                .FirstOrDefaultAsync(h => h.Id == hotelId);

            if (hotel != null)
            {
                var amenity = hotel.Amenities.FirstOrDefault(a => a.Id == amenityId);
                if (amenity != null)
                {
                    hotel.Amenities.Remove(amenity);
                }
            }
        }
    }
}
