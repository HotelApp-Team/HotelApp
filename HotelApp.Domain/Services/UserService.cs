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
            Roles = user.Roles.Select(r => r.Name).ToList(),
            CreatedAt = user.CreatedAt
        };
    }
}
