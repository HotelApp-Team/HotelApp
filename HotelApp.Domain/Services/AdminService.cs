using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System.Data;

namespace HotelApp.Domain.Services;

// AdminService sÄ±nÄ±fÄ±nÄ±n amacÄ±, admin paneli iÅŸ kurallarÄ±nÄ± yÃ¶netmektir.
// Dashboard verisi hazÄ±rlar, kullanÄ±cÄ±larÄ± listeler, aktiflik durumunu ve rolleri gÃ¼nceller.
// SOLID: SRP kullanÄ±r; sadece admin iÅŸ akÄ±ÅŸlarÄ±ndan sorumludur.
// SOLID: DIP kullanÄ±r; veri iÅŸlemleri iÃ§in somut repository yerine IUnitOfWork arayÃ¼zÃ¼ne baÄŸlÄ±dÄ±r.

public class AdminService : IAdminService
{
    // Log listesi sayfa basina maksimum kayit sayisi.
    // LoggerService'teki ayni sabitle bilincli olarak ikilenmistir;
    // boylece response meta ile DB sorgusunda ayni clamp kurali calisir.
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerService _logger;

    // UnitOfWork baÄŸÄ±mlÄ±lÄ±ÄŸÄ±nÄ± alÄ±r.
    public AdminService(IUnitOfWork unitOfWork, ILoggerService logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // E36 iÃ§in dashboard istatistiklerini toplar ve tek DTO halinde hazÄ±rlar.
    public async Task<AdminDashboardDto> GetDashboardDataAsync()
    {
        // Son 10 rezervasyonu veritabanÄ±ndan alÄ±yoruz.
        var recentReservations = await _unitOfWork.AdminRepository
            .GetRecentReservationsAsync(10);

        // Repository veriyi getirir, service ise API cevabÄ±nÄ± hazÄ±rlar.
        return new AdminDashboardDto
        {
            TotalHotels = await _unitOfWork.AdminRepository.GetTotalHotelsAsync(),
            TotalUsers = await _unitOfWork.AdminRepository.GetTotalUsersAsync(),
            TotalReservations = await _unitOfWork.AdminRepository.GetTotalReservationsAsync(),
            ActiveReservations = await _unitOfWork.AdminRepository.GetActiveReservationsAsync(),
            TotalRevenue = await _unitOfWork.AdminRepository.GetTotalRevenueAsync(),
            AverageRating = await _unitOfWork.AdminRepository.GetAverageRatingAsync(),

            // Reservation entity listesini API'de dÃ¶necek ReservationDto listesine Ã§eviriyoruz.
            RecentReservations = recentReservations
                .Select(ReservationService.MapToDto)
                .ToList()
        };
    }

    // E37 iÃ§in admin kullanÄ±cÄ± listesini hazÄ±rlar ve Ã¶zet sayaÃ§larla birlikte gÃ¼venli DTO olarak dÃ¶ndÃ¼rÃ¼r.
    public async Task<AdminUsersResponseDto> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize)
    {
        // Sayfalama deÄŸerleri hatalÄ±ysa DB sorgusuna gitmeden durduruyoruz.
        ValidatePaging(page, pageSize);

        // KullanÄ±cÄ±larÄ± filtrelenmiÅŸ ve sayfalanmÄ±ÅŸ ÅŸekilde AdminRepository'den alÄ±yoruz.
        var (users, totalCount) = await _unitOfWork.AdminRepository.GetPagedUsersForAdminAsync(search, isActive, page, pageSize);

        // Ã–zet kartlarÄ± sayfadaki 10 kayÄ±t yerine toplam kullanÄ±cÄ± havuzuna gÃ¶re hesaplanÄ±r.
        var (activeCount, passiveCount, adminCount) = await _unitOfWork.AdminRepository.GetUserSummaryForAdminAsync(search);

        // Entity listesini dÄ±ÅŸarÄ±ya aÃ§Ä±lacak gÃ¼venli DTO listesine Ã§eviriyoruz.
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

    // E38 iÃ§in admin panelinden kullanÄ±cÄ±nÄ±n aktiflik durumunu gÃ¼nceller.
    public async Task<AdminUserDto> UpdateUserStatusAsync(int userId, AdminUpdateUserStatusDto dto, int currentAdminId)
    {
        // IsActive zorunlu alan; null gelirse planÄ±n 400 davranÄ±ÅŸÄ±na uygun olarak durduruyoruz.
        if (dto == null || dto.IsActive == null)
            throw new ArgumentException("IsActive alanÄ± zorunludur.");

        // Status'u deÄŸiÅŸtirilecek kullanÄ±cÄ±yÄ± veritabanÄ±ndan getiriyoruz.
        var user = await _unitOfWork.AdminRepository.GetUserByIdAsync(userId);

        // KullanÄ±cÄ± yoksa controller tarafÄ±nda 404'e Ã§evrilecek hata fÄ±rlatÄ±yoruz.
        if (user == null)
            throw new KeyNotFoundException("KullanÄ±cÄ± bulunamadÄ±.");

        // Admin kendi hesabÄ±nÄ± pasife alamaz. Plan E38 bu durumda 400 dÃ¶ner.
        if (userId == currentAdminId && dto.IsActive is false)
            throw new ArgumentException("Admin kendi hesabÄ±nÄ± devre dÄ±ÅŸÄ± bÄ±rakamaz.");

        // Sadece IsActive alanÄ±nÄ± gÃ¼ncelliyoruz.
        user.IsActive = dto.IsActive.Value;

        // User EF tarafindan takip edildigi icin sadece UnitOfWork ile kaydediyoruz.
        await _unitOfWork.SaveChangesAsync();

        // GÃ¼ncel kullanÄ±cÄ± bilgisini gÃ¼venli DTO olarak dÃ¶ndÃ¼rÃ¼yoruz.
        return MapToAdminUserDto(user);
    }

    // E39 iÃ§in admin panelinden kullanÄ±cÄ±nÄ±n rollerini gÃ¼nceller.
    public async Task<AdminUserDto> UpdateUserRolesAsync(int userId, AdminUpdateUserRolesDto dto, int currentAdminId)
    {
        // Rol listesi boÅŸ gelirse controller tarafÄ±nda 400'e Ã§evrilecek hata fÄ±rlatÄ±yoruz.
        if (dto == null || dto.Roles == null)
            throw new ArgumentException("En az bir rol seÃ§ilmelidir.");

        // Gelen rol isimlerini temizliyoruz ve tekrar edenleri tekilleÅŸtiriyoruz.
        var requestedRoles = dto.Roles
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!requestedRoles.Any())
            throw new ArgumentException("En az bir rol seÃ§ilmelidir.");

        // RolÃ¼ deÄŸiÅŸtirilecek kullanÄ±cÄ±yÄ± veritabanÄ±ndan getiriyoruz.
        var user = await _unitOfWork.AdminRepository.GetUserByIdAsync(userId);

        // KullanÄ±cÄ± yoksa controller tarafÄ±nda 404'e Ã§evrilecek hata fÄ±rlatÄ±yoruz.
        if (user == null)
            throw new KeyNotFoundException("KullanÄ±cÄ± bulunamadÄ±.");

        // Role isimlerini DB'den getiriyoruz.
        var roles = await _unitOfWork.AdminRepository.GetRolesByNamesAsync(requestedRoles);

        // GÃ¶nderilen rol isimlerinden DB'de bulunmayan varsa 400 hatasÄ± fÄ±rlatÄ±yoruz.
        if (roles.Count != requestedRoles.Count)
            throw new ArgumentException("GeÃ§ersiz rol isimleri var.");

        if (userId == currentAdminId &&
            !requestedRoles.Contains("Admin", StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Admin kendi Admin rolÃ¼nÃ¼ kaldÄ±ramaz.");
        }

        // Plan kuralÄ±: GÃ¶nderilen rol listesi kullanÄ±cÄ±nÄ±n yeni rol listesi olur.
        user.Roles.Clear();

        // DB'den gelen rolleri kullanÄ±cÄ±ya ekliyoruz.
        foreach (var role in roles)
        {
            user.Roles.Add(role);
        }

        // User EF tarafindan takip edildigi icin sadece UnitOfWork ile kaydediyoruz.
        await _unitOfWork.SaveChangesAsync();

        // GÃ¼ncel kullanÄ±cÄ± bilgisini gÃ¼venli DTO olarak dÃ¶ndÃ¼rÃ¼yoruz.
        return MapToAdminUserDto(user);
    }

    public async Task<PagedResponseDto<LogDto>> GetLogsAsync(LogFilterDto filter)
    {
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

    public async Task<LogDto?> GetLogByIdAsync(int id)
    {
        return await _logger.GetLogByIdAsync(id);
    }

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

    public Task<ReservationDto> ConfirmReservationAsync(int reservationId, bool confirm)
    {
        var newStatus = confirm ? "Confirmed" : "Cancelled";
        return UpdateReservationStatusAsync(reservationId, newStatus);
    }
    // User entity'sini admin ekranÄ±na gÃ¼venli ÅŸekilde dÃ¶neceÄŸimiz AdminUserDto'ya Ã§evirir.
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

    // Page ve pageSize deÄŸerlerinin gÃ¼venli aralÄ±kta olup olmadÄ±ÄŸÄ±nÄ± kontrol eder.
    private static void ValidatePaging(int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("Page 1 veya daha bÃ¼yÃ¼k olmalÄ±dÄ±r.");

        if (pageSize < 1)
            throw new ArgumentException("PageSize 1 veya daha bÃ¼yÃ¼k olmalÄ±dÄ±r.");

        if (pageSize > 100)
            throw new ArgumentException("PageSize en fazla 100 olabilir.");
    }

    
}

