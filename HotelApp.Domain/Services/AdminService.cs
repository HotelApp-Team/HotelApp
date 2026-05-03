using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System.Data;

namespace HotelApp.Domain.Services;

// Amaç: Admin paneli iş kurallarını yönetir ve repository katmanından gelen veriyi DTO'ya çevirir.
// SOLID: SRP ile sadece admin iş akışlarından sorumludur; DIP ile IUnitOfWork arayüzüne bağlı çalışır.

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    // UnitOfWork bağımlılığını alır.
    public AdminService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

    // E37 için admin kullanıcı listesini hazırlar ve güvenli DTO olarak döndürür.
    public async Task<PagedResponseDto<AdminUserDto>> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize)
    {
        // Sayfalama değerleri hatalıysa DB sorgusuna gitmeden durduruyoruz.
        ValidatePaging(page, pageSize);

        // Kullanıcıları filtrelenmiş ve sayfalanmış şekilde AdminRepository'den alıyoruz.
        var (users, totalCount) = await _unitOfWork.AdminRepository.GetPagedUsersForAdminAsync(search, isActive, page, pageSize);

        // Entity listesini dışarıya açılacak güvenli DTO listesine çeviriyoruz.
        return new PagedResponseDto<AdminUserDto>
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = users.Select(MapToAdminUserDto).ToList()
        };
    }

    // E38 için admin panelinden kullanıcının aktiflik durumunu günceller.
    public async Task<AdminUserDto> UpdateUserStatusAsync(int userId, AdminUpdateUserStatusDto dto, int currentAdminId)
    {
        // Status'u değiştirilecek kullanıcıyı veritabanından getiriyoruz.
        var user = await _unitOfWork.AdminRepository.GetUserByIdAsync(userId);

        // Kullanıcı yoksa controller tarafında 404'e çevrilecek hata fırlatıyoruz.
        if (user == null)
            throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        // Admin kendi hesabını pasife alamaz. Bu güvenlik kuralı service katmanında durur.
        if (userId == currentAdminId && dto.IsActive == false)
            throw new InvalidOperationException("Admin kendi hesabını devre dışı bırakamaz.");

        // Sadece IsActive alanını güncelliyoruz.
        user.IsActive = dto.IsActive;

        // Değişikliği AdminRepository üzerinden işaretleyip UnitOfWork ile kaydediyoruz.
        _unitOfWork.AdminRepository.UpdateUser(user);
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

        // Değişikliği AdminRepository üzerinden işaretliyoruz.
        _unitOfWork.AdminRepository.UpdateUser(user);

        // Değişikliği kaydediyoruz.
        await _unitOfWork.SaveChangesAsync();

        // Güncel kullanıcı bilgisini güvenli DTO olarak döndürüyoruz.
        return MapToAdminUserDto(user);
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
