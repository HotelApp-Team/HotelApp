using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Services;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

// LoggerService, sistem loglarini Logs tablosuna yazan Singleton servistir.
// Singleton secilme nedeni: logger uygulamada cross-cutting concern, tek instance yeterli.
// DbContext Singleton ile uyumsuz oldugu icin her log cagrisinda kisa omurlu context acilir.
namespace HotelApp.Infrastructure.Services
{
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

        // Program.cs'den connection string'i bir kez set eder
        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }

        // INFO seviyesi log - sadece level'i ayarlayip helper'a delege eder
        public async Task LogInfoAsync(string message, int? userId = null, string? userEmail = null,
            string? controller = null, string? action = null, string? ipAddress = null)
        {
            await LogAsync("INFO", message, userId, userEmail, controller, action, ipAddress);
        }

        // WARNING seviyesi log
        public async Task LogWarningAsync(string message, int? userId = null, string? userEmail = null,
            string? controller = null, string? action = null, string? ipAddress = null)
        {
            await LogAsync("WARNING", message, userId, userEmail, controller, action, ipAddress);
        }

        // ERROR seviyesi log - exception detayini da alabilir
        public async Task LogErrorAsync(string message, int? userId = null, string? userEmail = null,
            string? controller = null, string? action = null, string? ipAddress = null,
            string? exceptionDetail = null)
        {
            await LogAsync("ERROR", message, userId, userEmail, controller, action, ipAddress, exceptionDetail);
        }

        // Sayfalama ile log listesi - admin panelinde gosterim icin
        public async Task<IEnumerable<LogDto>> GetLogsAsync(int pageNumber = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "LoggerService.Initialize(connectionString) cagrilmadi. Program.cs'i kontrol et.");

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

        // Toplam log sayisi - admin panelinde sayfalama UI'si icin (kac sayfa var?)
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
                Message = message,
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                UserEmail = userEmail,
                Controller = controller,
                Action = action,
                IpAddress = ipAddress,
                ExceptionDetail = exceptionDetail
            };

            await context.Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }
    }
}
