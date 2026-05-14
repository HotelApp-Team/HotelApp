using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System.Security.Cryptography;

namespace HotelApp.Domain.Services;

// AuthService sınıfının amacı, kullanıcı kimlik doğrulama işlemlerini yönetmektir.
// Kayıt, giriş, email doğrulama, doğrulama maili tekrar gönderme ve lockout kurallarını uygular.
// SOLID: SRP kullanır; sadece auth iş kurallarından sorumludur.
// SOLID: DIP kullanır; veri, token ve mail işlemleri için IUnitOfWork, IJwtService ve IEmailService arayüzlerine bağlıdır.

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly bool _useEmailVerification;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IEmailService emailService, bool useEmailVerification)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _emailService = emailService;
        _useEmailVerification = useEmailVerification;
    }

    // E01 için yeni kullanıcı kaydını oluşturur ve doğrulama akışını başlatır.
    public async Task<RegisterResponseDto> RegisterAsync(UserRegisterDto dto)
    {
        // Email zaten var mi?
        if (await _unitOfWork.AuthRepository.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Bu email zaten mevcut.");

        // Parolayi hashle
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Kullanici nesnesi olustur
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = hash,
            Phone = dto.Phone,
            IsActive = true
        };

        // Yeni kayit olan kullaniciya varsayilan olarak User rolunu atiyoruz.
        var defaultRole = await _unitOfWork.AuthRepository.GetRoleByNameAsync("User");
        if (defaultRole == null)
            throw new InvalidOperationException("Varsayılan Kullanıcı rolü bulunamadı.");

        user.Roles.Add(defaultRole);

        if (_useEmailVerification)
        {
            // Faz 2 akisi icin verify bilgilerini hazirla
            user.IsEmailVerified = false;
            user.EmailVerificationToken = Guid.NewGuid().ToString();
            user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

            // Kullaniciyi veritabanina ekle ve kaydi tamamla
            await _unitOfWork.AuthRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Mail gonderimi denenir; hata olursa kayit basarili kalir.
            // Kullanici daha sonra "Maili tekrar gonder" ile yeniden deneyebilir.
            // Controller bu sonucu log seviyesini secmek icin kullanir.
            var emailSent = await _emailService.SendVerificationEmailSafelyAsync(
                user.Email,
                user.FirstName,
                user.EmailVerificationToken!);

            return new RegisterResponseDto
            {
                RequiresEmailVerification = true,
                VerificationEmailSent = emailSent,
                Message = emailSent
                    ? "Kayıt başarılı! Lütfen emailinizi doğrulayın."
                    : "Kayıt başarılı, ancak doğrulama maili şu an gönderilemedi. Lütfen birkaç dakika sonra 'Maili tekrar gönder' ile yeniden deneyin."
            };

        }

        // Faz 1 fallback akisinda email verify beklenmez
        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.VerificationTokenExpiry = null;

        // Kullaniciyi veritabanina ekle
        await _unitOfWork.AuthRepository.AddAsync(user);

        // Degisiklikleri veritabanina kaydet
        await _unitOfWork.SaveChangesAsync();

        // JWT token olustur ve dondur
        var token = _jwtService.GenerateToken(user, false, out var expiresAt);

        return new RegisterResponseDto
        {
            RequiresEmailVerification = false,
            Message = "Kayıt başarılı.",
            Token = token,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            ExpiresAt = expiresAt
        };
    }

    // E02 için kullanıcı girişini kontrol eder ve JWT cevabını hazırlar.
    public async Task<LoginResponseDto> LoginAsync(UserLoginDto dto)
    {
        // Kullaniciyi email ile bul
        var user = await _unitOfWork.AuthRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Geçersiz email veya parola.");

        // Hesap aktif mi?
        if (!user.IsActive)
            throw new UnauthorizedAccessException("Hesabınız aktif değil.");

        // Email dogrulama acikken dogrulanmamis hesap giremez.
        // _useEmailVerification false oldugunda (Faz 1 fallback) bu kontrol atlanir ki
        // dogrulama zorunlulugu eklenmeden once kayit olmus kullanicilar kilitli kalmasin.
        if (_useEmailVerification && !user.IsEmailVerified)
            throw new UnauthorizedAccessException("Email adresiniz henüz doğrulanmamış. Lütfen emailinizi doğrulayın.");

        // Lockout suresi devam ediyor mu?
        if (user.LockoutEndTime.HasValue)
        {
            if (user.LockoutEndTime.Value > DateTime.UtcNow)
                throw new UnauthorizedAccessException("Hesabınız geçici olarak kilitlendi. Lütfen 1 dakika sonra tekrar deneyiniz");
            else
            {
                // Lockout suresi dolmussa sifirla
                user.FailedLoginAttempts = 0;
                user.LockoutEndTime = null;
            }
        }

        // Parolayi dogrula
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

        // Basarili giriste sayaci ve kilidi sifirla
        user.FailedLoginAttempts = 0;
        user.LockoutEndTime = null;

        await _unitOfWork.SaveChangesAsync();

        // JWT token olustur ve dondur
        var token = _jwtService.GenerateToken(user, dto.RememberMe, out var expiresAt);

        return new LoginResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            ExpiresAt = expiresAt
        };
    }

    // E42 için email doğrulama token'ını kontrol eder ve giriş cevabı hazırlar.
    public async Task<LoginResponseDto> VerifyEmailAsync(string token)
    {
        // Token bos mu kontrol et
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Doğrulama token'i boş olamaz.");

        // Token'a ait kullaniciyi veritabanindan bul
        var user = await _unitOfWork.AuthRepository.GetByVerificationTokenAsync(token);
        if (user == null)
            throw new KeyNotFoundException("Doğrulama token'ine ait kullanıcı bulunamadı.");

        // Token suresi gecmis mi kontrol et
        if (!user.VerificationTokenExpiry.HasValue || user.VerificationTokenExpiry.Value < DateTime.UtcNow)
            throw new InvalidOperationException("Doğrulama token'inin süresi dolmuş.");

        // Kullanicinin email bilgisini dogrulanmis yap ve token alanlarini temizle
        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.VerificationTokenExpiry = null;

        // Degisiklikleri veritabanina kaydet
        await _unitOfWork.SaveChangesAsync();

        // Otomatik giris icin JWT token uret
        var jwtToken = _jwtService.GenerateToken(user, false, out var expiresAt);

        // JWT ve kullanici bilgilerini LoginResponseDto olarak geri dondur
        return new LoginResponseDto
        {
            Token = jwtToken,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            ExpiresAt = expiresAt
        };
    }

    // E43 için doğrulama mailini yeniden göndermek üzere yeni token hazırlar.
    // Güvenlik için kullanıcı var/yok bilgisini dışarıya sızdırmaz.
    public async Task<ResendVerificationResultDto> ResendVerificationAsync(ResendVerificationDto dto)
    {
        const string genericMessage = "Eğer bu email adresi kayıtlı ve doğrulanmamışsa, doğrulama maili gönderildi.";

        var user = await _unitOfWork.AuthRepository.GetByEmailAsync(dto.Email);

        // Kullanici yoksa sessizce basarili gibi don (email kayitli mi bilgisi sizdirilmasin)
        if (user == null)
            return new ResendVerificationResultDto
            {
                Message = genericMessage,
                EmailSent = false,
                EmailSendAttempted = false
            };

        // Email zaten dogrulanmissa da ayni mesaji don (bu bilgi de disariya verilmemeli)
        if (user.IsEmailVerified)
            return new ResendVerificationResultDto
            {
                Message = genericMessage,
                EmailSent = false,
                EmailSendAttempted = false
            };

        // Yeni dogrulama token'i uret ve 24 saat gecerli yap
        user.EmailVerificationToken = Guid.NewGuid().ToString();
        user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

        await _unitOfWork.SaveChangesAsync();

        // Mail sonucu disa generic message ile cikilsa da controller warning/info log ayrimini bu alanla yapar.
        var emailSent = await _emailService.SendVerificationEmailSafelyAsync(
            user.Email,
            user.FirstName,
            user.EmailVerificationToken!);

        return new ResendVerificationResultDto
        {
            Message = genericMessage,
            EmailSent = emailSent,
            EmailSendAttempted = true
        };
    }

    // E54 için kullanıcıya şifre yenileme linki gönderilmesini sağlar.
    // Güvenlik için kullanıcı var/yok bilgisini dışarıya sızdırmaz.
    public async Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        const string genericMessage = "Eğer bu email sistemde kayıtlıysa şifre yenileme bağlantısı gönderildi.";

        var user = await _unitOfWork.AuthRepository.GetByEmailAsync(dto.Email);

        // Kullanici yoksa veya pasifse ayni mesaji doner; hesap varligi bilgisi sizdirilmez.
        if (user == null || !user.IsActive)
            return new ForgotPasswordResultDto
            {
                Message = genericMessage,
                EmailSendAttempted = false
            };

        // Kisa sure icinde tekrar reset maili istenirse yeni mail gondermeden generic mesaj doner.
        if (user.PasswordResetRequestedAt.HasValue && user.PasswordResetRequestedAt.Value > DateTime.UtcNow.AddMinutes(-2))
            return new ForgotPasswordResultDto
            {
                Message = genericMessage,
                EmailSendAttempted = false
            };

        // Sifre yenileme icin tek kullanimlik token bilgilerini hazirlar.
        user.PasswordResetToken = GeneratePasswordResetToken();
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
        user.PasswordResetRequestedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendPasswordResetEmailAsync(
            user.Email,
            user.FirstName,
            user.PasswordResetToken);

        return new ForgotPasswordResultDto
        {
            Message = genericMessage,
            EmailSendAttempted = true
        };
    }

    // E55 için reset token ile yeni şifreyi kaydeder ve token alanlarını temizler.
    public async Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto dto)
    {
        // Token bos mu kontrol eder.
        if (string.IsNullOrWhiteSpace(dto.Token))
            throw new ArgumentException("Şifre yenileme linki geçersiz veya süresi dolmuş.");

        // Yeni sifre tekrar kontrolu servis tarafinda da garanti edilir.
        if (dto.NewPassword != dto.ConfirmNewPassword)
            throw new ArgumentException("Şifreler uyuşmuyor.");

        var user = await _unitOfWork.AuthRepository.GetByPasswordResetTokenAsync(dto.Token);

        // Token DB'de yoksa veya kullanici pasifse ayni genel hata mesaji doner.
        if (user == null || !user.IsActive)
            throw new ArgumentException("Şifre yenileme linki geçersiz veya süresi dolmuş.");

        // Token suresi dolmus mu kontrol eder.
        if (!user.PasswordResetTokenExpiry.HasValue || user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            throw new ArgumentException("Şifre yenileme linki geçersiz veya süresi dolmuş.");

        // Yeni sifreyi BCrypt ile hashleyip kullaniciya kaydeder.
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        // Basarili sifre yenilemede reset token ve kilit bilgilerini temizler.
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        user.PasswordResetRequestedAt = null;
        user.FailedLoginAttempts = 0;
        user.LockoutEndTime = null;

        await _unitOfWork.SaveChangesAsync();

        return new ResetPasswordResultDto
        {
            Email = user.Email,
            Message = "Şifreniz başarıyla yenilendi. Yeni şifrenizle giriş yapabilirsiniz."
        };
    }

    // E54 için tahmin edilmesi zor, URL-safe reset token üretir.
    private static string GeneratePasswordResetToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}
