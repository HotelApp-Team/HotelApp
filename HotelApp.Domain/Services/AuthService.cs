using BCrypt.Net;
using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace HotelApp.Domain.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> RegisterAsync(UserRegisterDto dto)
    {
        // 1.Email zaten var mı?
        if (await _unitOfWork.AuthRepository.EmailExistsAsync(dto.Email))

            throw new InvalidOperationException("Bu email zaten mevcut.");
        // 2.Parolayı hashle
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // 3.Email dogrulama icin token ve sure olustur
        var verificationToken = Guid.NewGuid().ToString();
        var verificationTokenExpiry = DateTime.UtcNow.AddHours(24); // 24 saat geçerli

        //4.Kullanıcı nesnesi oluştur
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = hash,
            Phone = dto.Phone,
            IsActive = true,
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            VerificationTokenExpiry = verificationTokenExpiry
        };

        // 5.Yeni kayit olan kullaniciya varsayilan olarak User rolunu atiyoruz.
        var defaultRole = await _unitOfWork.AuthRepository.GetRoleByNameAsync("User");
        if (defaultRole == null)
        {
            throw new InvalidOperationException("Varsayılan Kullanıcı rolü bulunamadı.");
        }
        user.Roles.Add(defaultRole);

        // 6.Kullanıcıyı veritabanına kaydet.
        await _unitOfWork.AuthRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // 7.JWT token oluştur ve döndür.
        var token = _jwtService.GenerateToken(user, false, out var expiresAt);

        return new LoginResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            ExpiresAt = expiresAt
        };
    }
    public async Task<LoginResponseDto> LoginAsync(UserLoginDto dto)
    {
        // 1.Kullanıcıy mail ile bul
        var user = await _unitOfWork.AuthRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Geçersiz email veya parola.");

        // 2.Hesap aktif mi?
        if (!user.IsActive)
            throw new UnauthorizedAccessException("Hesabınız aktif değil.");

        // 3.Lockout süresi devam ediyor mu?
        if (user.LockoutEndTime.HasValue)
        {
            if (user.LockoutEndTime.Value > DateTime.UtcNow)
                throw new UnauthorizedAccessException("Hesabınız geçici olarak kilitlendi. Lütfen 1 dakika sonra tekrar deneyiniz");
            else
            {
                // Lockout süresi dolmuş, sıfırla
                user.FailedLoginAttempts = 0;
                user.LockoutEndTime = null;
            }
        }
        // 4.Parola doğrula
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEndTime = DateTime.UtcNow.AddMinutes(1); // 1 dakika kilitle
            }

            await _unitOfWork.SaveChangesAsync();

            if (user.LockoutEndTime.HasValue && user.LockoutEndTime.Value > DateTime.UtcNow)
                throw new UnauthorizedAccessException("5 başarısız deneme sonrası hesabınız 1 dakika kilitlendi.");

            throw new UnauthorizedAccessException("Geçersiz email veya parola.");
        }

        // 5. Başarılı girişte sayaç ve kilidi sıfırla
        user.FailedLoginAttempts = 0;
        user.LockoutEndTime = null;

        await _unitOfWork.SaveChangesAsync();

        // 4.JWT token oluştur ve döndür.
        var token = _jwtService.GenerateToken(user, dto.RememberMe, out var expiresAt);

        return new LoginResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            ExpiresAt = expiresAt
        };
    }
    // 6. Email doğrulama işlemi
    public async Task<LoginResponseDto> VerifyEmailAsync(string token)
    {
        // 1. Token boş mu kontrol et
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Doğrulama token'i boş olamaz.");

        // 2. Token'a ait kullanıcıyı veritabanından bul
        var user = await _unitOfWork.AuthRepository.GetByVerificationTokenAsync(token);
        if (user == null)
            throw new KeyNotFoundException("Doğrulama token'ine ait kullanıcı bulunamadı.");

        // 3. Token süresi geçmiş mi kontrol et
        if (!user.VerificationTokenExpiry.HasValue || user.VerificationTokenExpiry.Value < DateTime.UtcNow)
            throw new InvalidOperationException("Doğrulama token'inin süresi dolmuş.");

        // 4. Kullanıcının email bilgisini doğrulanmış yap ve token alanlarını temizle
        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.VerificationTokenExpiry = null;

        // 5. Değişiklikleri veritabanına kaydet
        await _unitOfWork.SaveChangesAsync();

        // 6. Otomatik giriş için JWT token üret
        var jwtToken = _jwtService.GenerateToken(user, false, out var expiresAt);

        // 7. JWT ve kullanıcı bilgilerini LoginResponseDto olarak geri döndür
        return new LoginResponseDto
        {
            Token = jwtToken,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            ExpiresAt = expiresAt
        };
    }

    // Email doğrulama mailini yeniden gönderme işlemi
    public async Task<string> ResendVerificationAsync(ResendVerificationDto dto)
    {
        // 1. Email ile kullanıcıyı bul
        var user = await _unitOfWork.AuthRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new KeyNotFoundException("Bu email adresine sahip bir kullanıcı bulunamadı.");
        // 2. Kullanıcının email doğrulanmış mı kontrol et
        if (user.IsEmailVerified)
            throw new InvalidOperationException("Bu email adresi zaten doğrulanmış.");
        // 3. Yeni bir doğrulama token'i oluştur ve kullanıcıya ata
        user.EmailVerificationToken = Guid.NewGuid().ToString();
        // 4. Token'in geçerlilik süresini belirle (örneğin 24 saat)
        user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
        // 5. Değişiklikleri veritabanına kaydet
        await _unitOfWork.SaveChangesAsync();
        // 6. Doğrulama mailini gönder (Bu kısımda gerçek mail gönderme kodu olmalı, burada sadece token'i döndürüyoruz)
        return user.EmailVerificationToken;
    }

}
