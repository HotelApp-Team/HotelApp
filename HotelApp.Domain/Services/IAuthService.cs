using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services
{
    // IAuthService arayüzünün amacı, auth işlemleri için servis sözleşmesi tanımlamaktır.
    // Kayıt, giriş, email doğrulama ve doğrulama maili tekrar gönderme metotlarını belirler.
    // SOLID: ISP kullanır; sadece auth servis ihtiyaçlarını içerir.
    // SOLID: DIP sağlar; controller somut AuthService yerine bu arayüze bağlı kalır.

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
