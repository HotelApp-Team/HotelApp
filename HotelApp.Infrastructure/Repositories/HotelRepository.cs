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
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelAppDbContext _context;

        public HotelRepository(HotelAppDbContext context)
        {
            _context = context;
        }

        // Tüm oteller
        public async Task<IEnumerable<Hotel>> GetAllAsync()
        {
            return await _context.Hotels
                .AsNoTracking()
                .ToListAsync();
        }

        // ID’ye göre otel
        public async Task<Hotel?> GetByIdAsync(int id)
        {
            return await _context.Hotels
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Detaylı otel (Include’lu)
        public async Task<Hotel?> GetWithDetailsAsync(int id)
        {
            return await _context.Hotels
                .Include(x => x.HotelImages)
                .Include(x => x.HotelRooms)
                .Include(x => x.Reviews)
                .Include(x => x.Amenities)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Şehre göre filtreleme
        public async Task<IEnumerable<Hotel>> GetByCityAsync(string city)
        {
            return await _context.Hotels
                .Where(x => x.City.ToLower() == city.ToLower())
                .AsNoTracking()
                .ToListAsync();
        }

        // Aktif oteller
        public async Task<IEnumerable<Hotel>> GetActiveAsync()
        {
            return await _context.Hotels
                .Where(x => x.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        // Yeni otel ekle
        public async Task AddAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
        }

        // Güncelle
        public void Update(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
        }

        // Sil
        public void Delete(Hotel hotel)
        {
            _context.Hotels.Remove(hotel);
        }
    }
}
