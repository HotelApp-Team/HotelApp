using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories
{
    // Amaç: Admin dashboard için gerekli sayım, toplam ve listeleme sorgularını veritabanından yapar.
    // SOLID: SRP ile sadece admin rapor sorgularından sorumludur; DIP ile IAdminRepository arayüzünü uygular.

    public class AdminRepository : IAdminRepository
    {
        private readonly HotelAppDbContext _context;

        // Veritabanı context bağımlılığını alır.
        public AdminRepository(HotelAppDbContext context)
        {
            _context = context;
        }

        // Toplam otel sayısını döndürür.
        public async Task<int> GetTotalHotelsAsync()
        {
            return await _context.Hotels.CountAsync();
        }

        // Toplam kullanıcı sayısını döndürür.
        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        // Toplam rezervasyon sayısını döndürür.
        public async Task<int> GetTotalReservationsAsync()
        {
            return await _context.Reservations.CountAsync();
        }

        // Onaylı rezervasyon sayısını döndürür.
        public async Task<int> GetActiveReservationsAsync()
        {
            return await _context.Reservations.CountAsync(x => x.Status == "Confirmed");
        }

        // Tamamlanmış rezervasyonlardan toplam geliri hesaplar.
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Reservations
                .Where(x => x.Status == "Completed")
                .SumAsync(x => (decimal?)x.TotalPrice) ?? 0m;
        }

        // Otellerin ortalama puanını hesaplar.
        public async Task<decimal> GetAverageRatingAsync()
        {
            return await _context.Hotels
                .AverageAsync(x => x.AvgRating) ?? 0m;
        }

        // En son oluşturulan rezervasyonları otel, oda tipi ve kullanıcı bilgileriyle listeler.
        public async Task<List<Reservation>> GetRecentReservationsAsync(int count)
        {
            return await _context.Reservations
                .Include(x => x.User)
                .Include(x => x.HotelRoom)
                    .ThenInclude(x => x.Hotel)
                .Include(x => x.HotelRoom)
                    .ThenInclude(x => x.RoomType)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
