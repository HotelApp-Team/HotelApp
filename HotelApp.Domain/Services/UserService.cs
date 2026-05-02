using HotelApp.Domain.DTOs;
using HotelApp.Domain.Interfaces;
using HotelApp.Domain.Entities;

// UserService, kullanici profilini getirme ve guncelleme islemlerini yonetir.
// Amaci, profil business logic'ini controller'dan ayirmaktir.
// SOLID: SRP kullanir, DIP ile IUnitOfWork abstraction'ina baglanir.
namespace HotelApp.Domain.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    // Constructor ile UnitOfWork'u aliyoruz, boylece repository'lere tek merkezden erisebiliyoruz
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> GetProfileAsync(int userId)
    {
        // 1. Kullaniciyi repository uzerinden ID ile getir
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

        // 2. Kullanici bulunamazsa yetkisiz erisim hatasi firlat
        if (user == null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // 3. Entity'yi API'de donulecek UserDto nesnesine cevir
        return MapToUserDto(user);
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, ProfileDto profileDto)
    {
        // 1. Guncellenecek kullaniciyi veritabanindan getir
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

        // 2. Kullanici yoksa hata firlat
        if (user == null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // 3. Profil bilgisinden gelen alanlarla entity'yi guncelle
        user.FirstName = profileDto.FirstName;
        user.LastName = profileDto.LastName;
        user.Phone = profileDto.Phone;

        // 4. Guncellenen entity'yi repository uzerinden isaretle ve degisiklikleri kaydet
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        // 5. Guncel profili UserDto olarak geri don
        return MapToUserDto(user);
    }

    // User entity'sini, disariya guvenli sekilde donecegimiz UserDto'ya mapler
    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
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

    // Admin kullanıcı listesini hazırlar ve güvenli DTO olarak döndürür.
    public async Task<PagedResponseDto<UserDto>> GetUsersForAdminAsync(string? search, bool? isActive, int page, int pageSize)
    {
        // Sayfalama değerleri hatalıysa DB sorgusuna gitmeden durduruyoruz.
        ValidatePaging(page, pageSize);

        // Kullanıcıları filtrelenmiş ve sayfalanmış şekilde repository'den alıyoruz.
        var (users, totalCount) = await _unitOfWork.UserRepository.GetPagedUsersForAdminAsync(search, isActive, page, pageSize);

        // Entity listesini dışarıya açılacak güvenli DTO listesine çeviriyoruz.
        return new PagedResponseDto<UserDto>
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Data = users.Select(MapToUserDto).ToList()
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
