using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Repositories
{
    // AdminRepository sınıfının amacı, admin paneli için gerekli veritabanı sorgularını yapmaktır.
    // Dashboard istatistikleri, kullanıcı listesi, kullanıcı rolleri ve aktiflik verilerini veritabanından getirir.
    // SOLID: SRP kullanır; sadece admin veri erişiminden sorumludur.
    // SOLID: DIP destekler; üst katmanlar bu sınıfı IAdminRepository arayüzü üzerinden kullanır.

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

        // Admin kullanıcı listesi için arama, aktiflik filtresi ve sayfalama uygular.
        public async Task<(List<User> Data, int TotalCount)> GetPagedUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize)
        {
            // Kullanıcı sorgusunu rollerle birlikte hazırlıyoruz.
            var query = _context.Users
                .Include(x => x.Roles)
                .AsNoTracking()
                .AsQueryable();

            // Search doluysa ad, soyad veya email içinde arama yapıyoruz.
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keywords = search.Trim();

                query = query.Where(x =>
                    x.FirstName.Contains(keywords) ||
                    x.LastName.Contains(keywords) ||
                    x.Email.Contains(keywords));
            }

            // isActive geldiyse sadece aktif veya pasif kullanıcıları getiriyoruz.
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            // Sayfalama cevabı için toplam kayıt sayısını alıyoruz.
            var totalCount = await query.CountAsync();

            // İstenen sayfadaki kullanıcıları en yeni kayıt önce olacak şekilde çekiyoruz.
            var users = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        // Admin kullanıcı özet kartları için sayfalamadan bağımsız toplamları hesaplar.
        public async Task<(int ActiveCount, int PassiveCount, int AdminCount)> GetUserSummaryForAdminAsync(string? search)
        {
            // Kullanıcı özetinde arama filtresi korunur, sayfalama ve aktiflik filtresi uygulanmaz.
            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

            // Search doluysa özet sayaçlar da aynı arama kapsamına göre hesaplanır.
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keywords = search.Trim();

                query = query.Where(x =>
                    x.FirstName.Contains(keywords) ||
                    x.LastName.Contains(keywords) ||
                    x.Email.Contains(keywords));
            }

            var activeCount = await query.CountAsync(x => x.IsActive);
            var passiveCount = await query.CountAsync(x => !x.IsActive);
            var adminCount = await query.CountAsync(x => x.Roles.Any(role => role.Name == "Admin"));

            return (activeCount, passiveCount, adminCount);
        }

        // Admin işlemleri için tek kullanıcıyı rolleriyle birlikte getirir.
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        // Role isimlerine göre rolleri getirir, admin kullanıcı düzenleme sırasında rollerin doğruluğunu kontrol etmek için kullanılır.
        public async Task<List<Role>> GetRolesByNamesAsync(List<string> roleNames)
        {
            return await _context.Roles
                .Where(x => roleNames.Contains(x.Name))
                .ToListAsync();
        }

        // Değişen kullanıcıyı güncellenecek olarak işaretler.
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<(List<Reservation> Data, int TotalCount)> GetPagedReservationsForAdminAsync(string? status, int? hotelId, DateTime? from, DateTime? to, int page, int pageSize)
        {
            // Admin ekranında kullanıcı, otel ve oda tipi adlarını göstermek için ilişkili tabloları dahil ediyoruz.
            var query = _context.Reservations
                .Include(x => x.User)
                .Include(x => x.HotelRoom).ThenInclude(x => x.Hotel)
                .Include(x => x.HotelRoom).ThenInclude(x => x.RoomType)
                .AsQueryable();

            // Durum filtresi geldiyse sadece o durumdaki rezervasyonları getirir.
            if (!string.IsNullOrWhiteSpace(status))
                query= query.Where(x => x.Status == status);

            // Otel filtresi geldiyse sadece ilgili otele ait rezervasyonları getirir.
            if (hotelId.HasValue)
                query = query.Where(x => x.HotelRoom.HotelId == hotelId.Value);

            // Başlangıç tarihi filtresi.
            if (from.HasValue)
                query = query.Where(x => x.CheckInDate >= DateOnly.FromDateTime(from.Value));

            // Bitiş tarihi filtresi.
            if (to.HasValue)
                query = query.Where(x => x.CheckOutDate <= DateOnly.FromDateTime(to.Value));

            var totalCount = await query.CountAsync();
           
            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }
    }
}
