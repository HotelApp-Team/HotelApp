using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services;

// UserService sınıfının amacı, kullanıcının kendi profil işlemlerini yönetmektir.
// Profil bilgisi getirme, profil güncelleme ve şifre değiştirme işlerini yapar.
// SOLID: SRP kullanır; sadece profil işlemlerinden sorumludur.
// SOLID: DIP kullanır; veri işlemleri için somut sınıfa değil IUnitOfWork arayüzüne bağlıdır.

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    // Constructor ile UnitOfWork'u aliyoruz, boylece repository'lere tek merkezden erisebiliyoruz.
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // E03 için giriş yapan kullanıcının profil bilgisini DTO olarak hazırlar.
    public async Task<UserDto> GetProfileAsync(int userId)
    {
        // 1. Kullaniciyi repository uzerinden ID ile getir.
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

        // 2. Kullanici bulunamazsa yetkisiz erisim hatasi firlat.
        if (user == null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // 3. Entity'yi API'de donulecek UserDto nesnesine cevir.
        return MapToUserDto(user);
    }

    // E46 için kullanıcının profil bilgilerini günceller ve güncel DTO döndürür.
    public async Task<UserDto> UpdateProfileAsync(int userId, ProfileDto profileDto)
    {
        // 1. Guncellenecek kullaniciyi veritabanindan getir.
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

        // 2. Kullanici yoksa hata firlat.
        if (user == null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // 3. Profil bilgisinden gelen alanlarla entity'yi guncelle.
        user.FirstName = profileDto.FirstName;
        user.LastName = profileDto.LastName;
        user.Phone = profileDto.Phone;

        // 4. Guncellenen entity'yi repository uzerinden isaretle ve degisiklikleri kaydet.
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        // 5. Guncel profili UserDto olarak geri don.
        return MapToUserDto(user);
    }

    // E47 için mevcut şifreyi kontrol eder ve yeni şifreyi güvenli şekilde kaydeder.
    public async Task ChangePasswordAsync(int userId, UserChangePasswordDto changePasswordDto)
    {
        // 1. Kullanici var mi kontrol et.
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

        if (user == null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // 2. Yeni sifre tekrar alaniyla ayni mi kontrol et.
        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            throw new InvalidOperationException("Yeni şifre ve tekrar şifre aynı olmalıdır.");

        // 3. Mevcut sifre dogru mu kontrol et.
        var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(
            changePasswordDto.CurrentPassword,
            user.PasswordHash);

        if (!isCurrentPasswordValid)
            throw new UnauthorizedAccessException("Mevcut şifre hatalı.");

        // 4. Yeni sifreyi hashleyip kaydet. Plain text sifre asla DB'ye yazilmaz.
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    // User entity'sini, disariya guvenli sekilde donecegimiz UserDto'ya mapler.
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
}
