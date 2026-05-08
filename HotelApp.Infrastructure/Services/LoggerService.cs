using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Services;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Services
{
    // LoggerService sınıfının amacı, sistem loglarını merkezi olarak yönetmektir.
    // Info, warning ve error loglarını Logs tablosuna yazar; admin için logları listeler ve sayar.
    // SOLID: SRP kullanır; sadece loglama işlemlerinden sorumludur.
    // Design Pattern: Singleton kullanır; uygulama boyunca tek logger instance'ı ile çalışır.

    public class LoggerService : ILoggerService
    {
        // Connection string Initialize() ile Program.cs'den alinir
        private static string? _connectionString;

        // Lazy<T> ile thread-safe Singleton instance olusturma
        private static readonly Lazy<LoggerService> _instance =
            new(() => new LoggerService());

        // Disaridan tek erisim noktasi
        public static LoggerService Instance => _instance.Value;

        // Private constructor: disaridan new ile uretilemez
        private LoggerService() { }

        // DB'deki Message ve ExceptionDetail kolonlarinin en fazla kac karakter tutabildigi.
        // Log yazmadan once uzun metinleri bu degerlere gore kisaltiyoruz ki DB hata vermesin.
        private const int MessageMaxLength = 2000;
        private const int ExceptionDetailMaxLength = 4000;

        // Sayfalama icin guvenlik sinirlari - hatali veya kotu niyetli parametrelere karsi koruma.
        private const int MaxPageSize = 100;

        // Program.cs'den connection string'i bir kez set eder
        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }

        // E44 log listesinde görünecek INFO kaydını oluşturur.
        public async Task LogInfoAsync(string message, int? userId = null, string? userEmail = null,
            string? controller = null, string? action = null, string? ipAddress = null)
        {
            await LogAsync("INFO", message, userId, userEmail, controller, action, ipAddress);
        }

        // E44 log listesinde görünecek WARNING kaydını oluşturur.
        public async Task LogWarningAsync(string message, int? userId = null, string? userEmail = null,
            string? controller = null, string? action = null, string? ipAddress = null)
        {
            await LogAsync("WARNING", message, userId, userEmail, controller, action, ipAddress);
        }

        // E44 ve E45 için hata logunu exception detayıyla kaydeder.
        public async Task LogErrorAsync(string message, int? userId = null, string? userEmail = null,
            string? controller = null, string? action = null, string? ipAddress = null,
            string? exceptionDetail = null)
        {
            await LogAsync("ERROR", message, userId, userEmail, controller, action, ipAddress, exceptionDetail);
        }

        // E44 icin sistem loglarini filtreli ve sayfali sekilde getirir.
        // Filtre alanlarinin bos/null olanlari atlanir, sadece dolu olanlar Where olarak uygulanir.
        public async Task<IEnumerable<LogDto>> GetLogsAsync(LogFilterDto filter)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "LoggerService.Initialize(connectionString) cagrilmadi. Program.cs'i kontrol et.");

            // Sayfalama parametrelerini guvenli araliga cek.
            // PageNumber 0 veya negatif gelirse Skip negatif olur ve sorgu patlar;
            // cok buyuk PageSize (orn. 99999) ise DB'den devasa sonuc cekip sunucuyu yorar.
            // DTO'nun kendisine dokunmuyoruz, sadece lokal kopyalari kullaniyoruz.
            var pageNumber = Math.Max(1, filter.PageNumber);
            var pageSize = Math.Clamp(filter.PageSize, 1, MaxPageSize);

            var options = new DbContextOptionsBuilder<HotelAppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new HotelAppDbContext(options);

            // IQueryable ile basla, filtreleri ortak helper'da uygula, sonra sirala-sayfala-projection
            var query = context.Logs.AsQueryable();
            query = ApplyFilter(query, filter);

            // En yeni log en ustte, sayfalama uygula, entity -> DTO mapping
            return await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LogDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Level = l.Level,
                    Message = l.Message,
                    UserId = l.UserId,
                    UserEmail = l.UserEmail,
                    Controller = l.Controller,
                    Action = l.Action,
                    IpAddress = l.IpAddress,
                    ExceptionDetail = l.ExceptionDetail
                })
                .ToListAsync();
        }

        // E44 icin filtreye uyan toplam log sayisini getirir.
        // GetLogsAsync ile ayni filtreyi paylasir; aksi halde sayfalamadaki "toplam" yanlis cikar.
        public async Task<int> GetLogCountAsync(LogFilterDto filter)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "LoggerService.Initialize(connectionString) cagrilmadi. Program.cs'i kontrol et.");

            var options = new DbContextOptionsBuilder<HotelAppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new HotelAppDbContext(options);

            var query = context.Logs.AsQueryable();
            query = ApplyFilter(query, filter);

            return await query.CountAsync();
        }

        // E45 için id'ye göre tek log kaydını getirir.
        // Bulunamazsa null doner; controller bunu 404 NotFound olarak verir.
        public async Task<LogDto?> GetLogByIdAsync(int id)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "LoggerService.Initialize(connectionString) cagrilmadi. Program.cs'i kontrol et.");

            var options = new DbContextOptionsBuilder<HotelAppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new HotelAppDbContext(options);

            var log = await context.Logs.FindAsync(id);
            if (log == null)
                return null;

            return new LogDto
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                Level = log.Level,
                Message = log.Message,
                UserId = log.UserId,
                UserEmail = log.UserEmail,
                Controller = log.Controller,
                Action = log.Action,
                IpAddress = log.IpAddress,
                ExceptionDetail = log.ExceptionDetail
            };
        }

        // Private helper - tum public log metodlari buraya delege eder.
        // Boylece DB yazma kodu tek yerde, kopya yok (DRY prensibi).
        private async Task LogAsync(
            string level,
            string message,
            int? userId,
            string? userEmail,
            string? controller,
            string? action,
            string? ipAddress,
            string? exceptionDetail = null)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "LoggerService.Initialize(connectionString) cagrilmadi. Program.cs'i kontrol et.");

            // Bu cagri icin yeni, kisa omurlu DbContext olustur.
            // Singleton + uzun omurlu context = thread safety sorunu, bu yuzden her cagrida yenisi acilir.
            var options = new DbContextOptionsBuilder<HotelAppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new HotelAppDbContext(options);

            var log = new Log
            {
                Level = level,
                // Cok uzun mesaj/exception DB'ye yazilirken hata verebilir
                // (kolon limitini asar). Bu yuzden kayit oncesi kisaltiyoruz.
                Message = Truncate(message, MessageMaxLength)!,
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                UserEmail = userEmail,
                Controller = controller,
                Action = action,
                IpAddress = ipAddress,
                ExceptionDetail = Truncate(exceptionDetail, ExceptionDetailMaxLength)
            };

            await context.Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }

        // Filtre DTO'sundaki dolu alanlara gore IQueryable'a Where sartlari ekler.
        // Bos veya null filtreler atlanir, boylece istemci tek tek filtre secebilir.
        // Hem GetLogsAsync hem GetLogCountAsync ayni mantigi paylasir (DRY).
        private IQueryable<Log> ApplyFilter(IQueryable<Log> query, LogFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Level))
                query = query.Where(l => l.Level == filter.Level);

            if (filter.DateFrom.HasValue)
                query = query.Where(l => l.Timestamp >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(l => l.Timestamp <= filter.DateTo.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(l => l.Message.Contains(filter.Search));

            return query;
        }

        // Metni maxLength'ten uzunsa keser, degilse oldugu gibi birakir. Null/bos girdi dokunmadan donulur.
        private static string? Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;
            return value.Substring(0, maxLength);
        }
    }
}
