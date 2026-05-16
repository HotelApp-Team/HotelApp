using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System.Data;

namespace HotelApp.Domain.Services;

// AdminService sınıfının amacı, admin paneli iş kurallarını yönetmektir.
// Dashboard verisi hazırlar, kullanıcıları listeler, aktiflik durumunu ve rolleri günceller.
// SOLID: SRP kullanır; sadece admin iş akışlarından sorumludur.
// SOLID: DIP kullanır; veri işlemleri için somut repository yerine IUnitOfWork arayüzüne bağlıdır.

public class AdminService : IAdminService
{
    // Log listesi sayfa basina maksimum kayit sayisi.
    // LoggerService'teki ayni sabitle bilincli olarak ikilenmistir;
    // boylece response meta ile DB sorgusunda ayni clamp kurali calisir.
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerService _logger;

    // UnitOfWork bağımlılığını alır.
    public AdminService(IUnitOfWork unitOfWork, ILoggerService logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // E36 için dashboard istatistiklerini toplar ve tek DTO halinde hazırlar.
    public async Task<AdminDashboardDto> GetDashboardDataAsync()
    {
        // Son 10 rezervasyonu veritabanından alıyoruz.
        var recentReservations = await _unitOfWork.AdminRepository
            .GetRecentReservationsAsync(10);

        // Repository veriyi getirir, service ise API cevabını hazırlar.
        return new AdminDashboardDto
        {
            TotalHotels = await _unitOfWork.AdminRepository.GetTotalHotelsAsync(),
            TotalUsers = await _unitOfWork.AdminRepository.GetTotalUsersAsync(),
            TotalReservations = await _unitOfWork.AdminRepository.GetTotalReservationsAsync(),
            ActiveReservations = await _unitOfWork.AdminRepository.GetActiveReservationsAsync(),
            TotalRevenue = await _unitOfWork.AdminRepository.GetTotalRevenueAsync(),
            AverageRating = await _unitOfWork.AdminRepository.GetAverageRatingAsync(),

            // Reservation entity listesini API'de dönecek ReservationDto listesine çeviriyoruz.
            RecentReservations = recentReservations
                .Select(ReservationService.MapToDto)
                .ToList()
        };
    }

    // E37 için admin kullanıcı listesini hazırlar ve özet sayaçlarla birlikte güvenli DTO olarak döndürür.
    public async Task<AdminUsersResponseDto> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize)
    {
        // Sayfalama değerleri hatalıysa DB sorgusuna gitmeden durduruyoruz.
        ValidatePaging(page, pageSize);

        // Kullanıcıları filtrelenmiş ve sayfalanmış şekilde AdminRepository'den alıyoruz.
        var (users, totalCount) = await _unitOfWork.AdminRepository.GetPagedUsersForAdminAsync(search, isActive, page, pageSize);

        // Özet kartları sayfadaki 10 kayıt yerine toplam kullanıcı havuzuna göre hesaplanır.
        var (activeCount, passiveCount, adminCount) = await _unitOfWork.AdminRepository.GetUserSummaryForAdminAsync(search);

        // Entity listesini dışarıya açılacak güvenli DTO listesine çeviriyoruz.
        return new AdminUsersResponseDto
        {
            TotalCount = totalCount,
            ActiveCount = activeCount,
            PassiveCount = passiveCount,
            AdminCount = adminCount,
            Page = page,
            PageSize = pageSize,
            Data = users.Select(MapToAdminUserDto).ToList()
        };
    }

    // E38 için admin panelinden kullanıcının aktiflik durumunu günceller.
    public async Task<AdminUserDto> UpdateUserStatusAsync(int userId, AdminUpdateUserStatusDto dto, int currentAdminId)
    {
        // IsActive zorunlu alan; null gelirse planın 400 davranışına uygun olarak durduruyoruz.
        if (dto == null || dto.IsActive == null)
            throw new ArgumentException("IsActive alanı zorunludur.");

        // Status'u değiştirilecek kullanıcıyı veritabanından getiriyoruz.
        var user = await _unitOfWork.AdminRepository.GetUserByIdAsync(userId);

        // Kullanıcı yoksa controller tarafında 404'e çevrilecek hata fırlatıyoruz.
        if (user == null)
            throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        // Admin kendi hesabını pasife alamaz. Plan E38 bu durumda 400 döner.
        if (userId == currentAdminId && dto.IsActive is false)
            throw new ArgumentException("Admin kendi hesabını devre dışı bırakamaz.");

        // Sadece IsActive alanını güncelliyoruz.
        user.IsActive = dto.IsActive.Value;

        // User EF tarafindan takip edildigi icin sadece UnitOfWork ile kaydediyoruz.
        await _unitOfWork.SaveChangesAsync();

        // Güncel kullanıcı bilgisini güvenli DTO olarak döndürüyoruz.
        return MapToAdminUserDto(user);
    }

    // E39 için admin panelinden kullanıcının rollerini günceller.
    public async Task<AdminUserDto> UpdateUserRolesAsync(int userId, AdminUpdateUserRolesDto dto, int currentAdminId)
    {
        // Rol listesi boş gelirse controller tarafında 400'e çevrilecek hata fırlatıyoruz.
        if (dto == null || dto.Roles == null)
            throw new ArgumentException("En az bir rol seçilmelidir.");

        // Gelen rol isimlerini temizliyoruz ve tekrar edenleri tekilleştiriyoruz.
        var requestedRoles = dto.Roles
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!requestedRoles.Any())
            throw new ArgumentException("En az bir rol seçilmelidir.");

        // Rolü değiştirilecek kullanıcıyı veritabanından getiriyoruz.
        var user = await _unitOfWork.AdminRepository.GetUserByIdAsync(userId);

        // Kullanıcı yoksa controller tarafında 404'e çevrilecek hata fırlatıyoruz.
        if (user == null)
            throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        // Role isimlerini DB'den getiriyoruz.
        var roles = await _unitOfWork.AdminRepository.GetRolesByNamesAsync(requestedRoles);

        // Gönderilen rol isimlerinden DB'de bulunmayan varsa 400 hatası fırlatıyoruz.
        if (roles.Count != requestedRoles.Count)
            throw new ArgumentException("Geçersiz rol isimleri var.");

        if (userId == currentAdminId &&
            !requestedRoles.Contains("Admin", StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Admin kendi Admin rolünü kaldıramaz.");
        }

        // Plan kuralı: Gönderilen rol listesi kullanıcının yeni rol listesi olur.
        user.Roles.Clear();

        // DB'den gelen rolleri kullanıcıya ekliyoruz.
        foreach (var role in roles)
        {
            user.Roles.Add(role);
        }

        // User EF tarafindan takip edildigi icin sadece UnitOfWork ile kaydediyoruz.
        await _unitOfWork.SaveChangesAsync();

        // Güncel kullanıcı bilgisini güvenli DTO olarak döndürüyoruz.
        return MapToAdminUserDto(user);
    }

    // E44 için LoggerService'in iki çağrısını PagedResponseDto zarfında birleştirir.
    public async Task<PagedResponseDto<LogDto>> GetLogsAsync(LogFilterDto filter)
    {
        // LoggerService zaten Math.Clamp ile sayfa parametrelerini koruyor.
        // Burada da ayni clamp'i tekrarliyoruz ki response meta ile DB sorgusu
        // ayni sayilari gostersin (orn. PageSize=999 istense bile meta 100 doner).
        var safePage = Math.Max(1, filter.PageNumber);
        var safePageSize = Math.Clamp(filter.PageSize, 1, MaxPageSize);

        var logs = await _logger.GetLogsAsync(filter);
        var totalCount = await _logger.GetLogCountAsync(filter);

        return new PagedResponseDto<LogDto>
        {
            TotalCount = totalCount,
            Page = safePage,
            PageSize = safePageSize,
            Data = logs.ToList()
        };
    }

    // E45 için tek log kaydını ileterek logger katmanına delege ediyoruz.
    public async Task<LogDto?> GetLogByIdAsync(int id)
    {
        return await _logger.GetLogByIdAsync(id);
    }

    // Admin paneli icin rezervasyonlari filtreli ve sayfali sekilde getirir.
    public async Task<PagedResponseDto<ReservationDto>> GetReservationsAsync(string? status, int? hotelId, DateTime? from, DateTime? to, int page, int pageSize)
    {
        ValidatePaging(page, pageSize);

        var (reservations, totalCount) =
            await _unitOfWork.AdminRepository.GetPagedReservationsForAdminAsync(
                status, hotelId, from, to, page, pageSize);

        return new PagedResponseDto<ReservationDto>
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = reservations.Select(ReservationService.MapToDto).ToList()
        };
    }

    // Admin panelinden rezervasyon durumunu gunceller. Sadece Confirmed, Cancelled veya Completed kabul edilir.
    public async Task<ReservationDto> UpdateReservationStatusAsync(int reservationId, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Durum zorunludur.");

        var allowedStatuses = new[] { "Confirmed", "Cancelled", "Completed" };
        if (!allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Gecersiz rezervasyon durumu.");

        var reservation = await _unitOfWork.ReservationRepository.GetWithDetailsAsync(reservationId);

        if (reservation == null)
            throw new KeyNotFoundException("Rezervasyon bulunamadi.");

        reservation.Status = status.Trim();
        _unitOfWork.ReservationRepository.Update(reservation);
        await _unitOfWork.SaveChangesAsync();

        return ReservationService.MapToDto(reservation);
    }

    // Admin panelinden rezervasyonu onaylar (Confirmed) veya reddeder (Cancelled).
    public Task<ReservationDto> ConfirmReservationAsync(int reservationId, bool confirm)
    {
        var newStatus = confirm ? "Confirmed" : "Cancelled";
        return UpdateReservationStatusAsync(reservationId, newStatus);
    }

    // User entity'sini admin ekranına güvenli şekilde döneceğimiz AdminUserDto'ya çevirir.
    private static AdminUserDto MapToAdminUserDto(User user)
    {
        return new AdminUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            IsActive = user.IsActive,
            Roles = user.Roles.Select(r => r.Name).ToList(),
            CreatedAt = user.CreatedAt
        };
    }

    // Page ve pageSize değerlerinin güvenli aralıkta olup olmadığını kontrol eder.
    private static void ValidatePaging(int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("Page 1 veya daha büyük olmalıdır.");

        if (pageSize < 1)
            throw new ArgumentException("PageSize 1 veya daha büyük olmalıdır.");

        if (pageSize > 100)
            throw new ArgumentException("PageSize en fazla 100 olabilir.");
    }

    
}

