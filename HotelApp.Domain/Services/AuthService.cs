using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

// AuthService, kayit, login, email verify ve resend gibi kimlik islemlerini yonetir.
// Amaci, auth kurallarini tek yerde toplamak ve controller'i sade tutmaktir.
// SOLID: SRP kullanir, DIP ile IUnitOfWork/IJwtService/IEmailService abstraction'larina baglanir.
namespace HotelApp.Domain.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly ILoggerService _logger;
    private readonly bool _useEmailVerification;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IEmailService emailService, ILoggerService logger, bool useEmailVerification)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _emailService = emailService;
        _logger = logger;
        _useEmailVerification = useEmailVerification;
    }

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
            var emailSent = await TrySendVerificationEmailAsync(user, nameof(RegisterAsync));

            return new RegisterResponseDto
            {
                RequiresEmailVerification = true,
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

    // Email dogrulama islemi
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

    // Dogrulama mailini yeniden gonderir.
    // Guvenlik notu: Saldirgan hangi email'lerin sistemde kayitli oldugunu ogrenmesin diye
    // her durumda (kullanici var/yok, dogrulanmis/dogrulanmamis) ayni mesaji donuyoruz.
    public async Task<string> ResendVerificationAsync(ResendVerificationDto dto)
    {
        const string genericMessage = "Eğer bu email adresi kayıtlı ve doğrulanmamışsa, doğrulama maili gönderildi.";

        var user = await _unitOfWork.AuthRepository.GetByEmailAsync(dto.Email);

        // Kullanici yoksa sessizce basarili gibi don (email kayitli mi bilgisi sizdirilmasin)
        if (user == null)
            return genericMessage;

        // Email zaten dogrulanmissa da ayni mesaji don (bu bilgi de disariya verilmemeli)
        if (user.IsEmailVerified)
            return genericMessage;

        // Yeni dogrulama token'i uret ve 24 saat gecerli yap
        user.EmailVerificationToken = Guid.NewGuid().ToString();
        user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

        await _unitOfWork.SaveChangesAsync();

        await TrySendVerificationEmailAsync(user, nameof(ResendVerificationAsync));

        return genericMessage;
    }

    // Mail gonderimini dener; hata olursa loglar ve false doner.
    private async Task<bool> TrySendVerificationEmailAsync(User user, string actionName)
    {
        try
        {
            await _emailService.SendVerificationEmailAsync(
                user.Email,
                user.FirstName,
                user.EmailVerificationToken!);

            return true;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(
                message: "Doğrulama maili gönderilemedi.",
                userId: user.Id,
                userEmail: user.Email,
                controller: nameof(AuthService),
                action: actionName,
                exceptionDetail: ex.ToString());

            return false;
        }
    }
}
