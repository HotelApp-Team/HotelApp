using BCrypt.Net;
using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HotelApp.Domain.Services
{
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

            // 3.Kullanıcı entity'sini oluştur.
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = hash,
                Phone = dto.Phone,
                IsActive = true,
                IsEmailVerified = false,
                FailedLoginAttempts = 0,
                CreatedAt = DateTime.UtcNow
            };

            // 4.Kullanıcıyı veritabanına kaydet.
            await _unitOfWork.AuthRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // 5.JWT token oluştur ve döndür.
            var token = _jwtService.GenerateToken(user, out var expiresAt);

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

            // 3.Parola doğrula
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Geçersiz email veya parola.");

            // 4.JWT token oluştur ve döndür.
            var token = _jwtService.GenerateToken(user, out var expiresAt);

            return new LoginResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                ExpiresAt = expiresAt

            };
        }
    }
}
