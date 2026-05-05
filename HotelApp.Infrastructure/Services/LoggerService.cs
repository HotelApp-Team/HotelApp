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
        private const int DefaultPageSize = 20;
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

        // E44 için sistem loglarını sayfalı şekilde getirir.
        public async Task<IEnumerable<LogDto>> GetLogsAsync(int pageNumber = 1, int pageSize = DefaultPageSize)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "LoggerService.Initialize(connectionString) cagrilmadi. Program.cs'i kontrol et.");

            // Sayfalama parametrelerini guvenli araliga cekiyoruz.
            // pageNumber 0 veya negatif gelirse Skip negatif olur ve sorgu patlar;
            // cok buyuk pageSize (orn. 99999) ise DB'den devasa sonuc cekip sunucuyu yorar.
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

            var options = new DbContextOptionsBuilder<HotelAppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new HotelAppDbContext(options);

            // En yeni log en ustte, sayfalama uygula, entity -> DTO mapping
            return await context.Logs
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

        // E44 için toplam log sayısını getirir.
        public async Task<int> GetLogCountAsync()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "LoggerService.Initialize(connectionString) cagrilmadi. Program.cs'i kontrol et.");

            var options = new DbContextOptionsBuilder<HotelAppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new HotelAppDbContext(options);

            return await context.Logs.CountAsync();
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

        // Metni maxLength'ten uzunsa keser, degilse oldugu gibi birakir. Null/bos girdi dokunmadan donulur.
        private static string? Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;
            return value.Substring(0, maxLength);
        }
    }
}
