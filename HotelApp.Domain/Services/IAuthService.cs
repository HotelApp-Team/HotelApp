using HotelApp.Domain.DTOs;

// IAuthService, auth islemleri icin servis sozlesmesini tanimlar.
// Amaci, controller'in somut sinifa degil interface'e baglanmasini saglamaktir.
// SOLID: DIP ve ISP kullanir.
namespace HotelApp.Domain.Services
{
    public interface IAuthService
    {
        // Yeni kullanici kaydi olusturur ve Faz ayarina gore verify mesaji veya JWT dondurur
        Task<RegisterResponseDto> RegisterAsync(UserRegisterDto registerRequest);

        // Kullanici girisi yapar, bilgiler dogruysa JWT token iceren response dondurur
        Task<LoginResponseDto> LoginAsync(UserLoginDto loginRequest);

        // Email dogrulama token'ini kontrol eder, kullaniciyi dogrular ve otomatik giris icin JWT dondurur
        Task<LoginResponseDto> VerifyEmailAsync(string token);

        // Email dogrulama mailini yeniden gondermek icin yeni verification token uretir.
        // Controller'in log seviyesi secebilmesi icin hem generic mesaji hem de mail sonucunu doner.
        Task<ResendVerificationResultDto> ResendVerificationAsync(ResendVerificationDto dto);
    }
}
