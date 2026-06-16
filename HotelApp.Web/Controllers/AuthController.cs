using HotelApp.Web.Helpers;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// AuthController sınıfının amacı, web tarafındaki auth sayfalarını yönetmektir.
// Login, register, email doğrulama, doğrulama maili tekrar gönderme ve logout akışlarını API ile konuşturur.
// SOLID: SRP kullanır; sadece web auth sayfa akışlarından sorumludur.

public class AuthController : Controller
{
    // API ile konuşmak için ApiService'i kullanacağız.
    // DI ile Program.cs'te kaydedildiği için otomatik olarak sağlanır.
    private readonly ApiService _apiService;
    // IConfiguration JWT imza/issuer/audience'a IsAuthenticated icinde erismek icin gerekli.
    private readonly IConfiguration _configuration;

    public AuthController(ApiService apiService, IConfiguration configuration)
    {
        _apiService = apiService;
        _configuration = configuration;
    }

    // Giriş sayfasını açar.
    // Kullanıcı korumalı sayfalara erişmeye çalıştığında buraya yönlendirilir.
    // Sistem onu Login'e yonlendirirken URL'ye ?returnUrl=/Reservations/My ekler.
    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        // Tarayicinin Login sayfasini cache'lemesini engelle.
        // Yoksa kullanici onceki hatali submit'ten sonra geri gelince eski "Gecersiz email" mesajini gorebilir.
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        // Zaten giriş yapmışsa: ReturnUrl varsa oraya (sadece site içi adresse), yoksa ana sayfaya yönlendir.
        if (HttpContext.Session.IsAuthenticated(_configuration))
        {
            // Url.IsLocalUrl: Open redirect saldirisi engeli (https://kotusite.com gibi disari yonlendirilmesin).
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // Register'dan gelen başarı mesajı varsa view'e taşıyalım.
        if (TempData["SuccessMessage"] is string successMessage)
        {
            ViewBag.SuccessMessage = successMessage;
        }

        // Oturum sona erme gibi bilgilendirme mesajlarini view'e tasi (orn. 401 sonrasi).
        if (TempData["InfoMessage"] is string infoMessage)
        {
            ViewBag.InfoMessage = infoMessage;
        }

        // Form modeline hero istatistikleri de eklenir; sayilar API endpointinden gelir.
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl,
            HeroStats = await GetHeroStatsAsync()
        });
    }

    // Login formu gönderildiğinde çalışır.
    // [ValidateAntiForgeryToken] form sahteciliğine karşı koruma sağlar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Validation kuralları sağlanmıyorsa formu hata mesajıyla geri gösterir.
        if (!ModelState.IsValid)
        {
            model.HeroStats = await GetHeroStatsAsync();
            return View(model);
        }

        // API'ye göndermek için sadece gerekli alanları paketliyoruz.
        // ReturnUrl ilgilendirmez, dışarda bırakıyoruz.
        var loginData = new
        {
            Email = model.Email,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        //API'ye POST isteği gönderiyoruz.
        var response = await _apiService.PostAsync<LoginResponse>("auth/login", loginData);

        // Başarısızsa hata mesajı ile formu geri gösterir.
        // API'den gelen mesajı ModelState'in genel hata listesine ekle.
        // View tarafında @Html.ValidationSummary() ile gösterilir.
        if (!response.Success)
        {
            ModelState.AddModelError(string.Empty, response.Message);
            model.HeroStats = await GetHeroStatsAsync();
            return View(model);
        }

        // Başarılı API'nin döndürdüğü token ve kullanıcı bilgilerini sessiona yaz.
        // API service her istekte Session'dan token'ı okuyarak Authorization header'ına ekleyecek.
        var data = response.Data!;
        HttpContext.Session.SetString("JWToken", data.Token);
        HttpContext.Session.SetString("UserEmail", data.Email);
        HttpContext.Session.SetString("UserFullName", data.FullName);
        SetRememberMeCookie(data, model.RememberMe);

        // ReturnUrl varsa oraya yönlendir, yoksa ana sayfaya.
        // Url.IsLocalUrl ile güvenli bir yönlendirme yapıyoruz, dışarıya yönlendirmiyoruz.
        // kotu niyetli biri ?returnUrl=https://kotu-site.com gonderirse engellenir.
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    // Şifremi unuttum sayfasını açar.
    [HttpGet]
    public async Task<IActionResult> ForgotPassword()
    {
        // Giriş yapmış kullanıcı şifre yenileme istemek yerine ana sayfaya yönlendirilir.
        if (HttpContext.Session.IsAuthenticated(_configuration))
            return RedirectToAction("Index", "Home");

        // Form modeline hero istatistikleri de eklenir; tasarım login/register ile aynı kalır.
        return View(new ForgotPasswordViewModel
        {
            HeroStats = await GetHeroStatsAsync()
        });
    }

    // Şifremi unuttum formu gönderildiğinde API'den reset linki maili ister.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        // Validation kuralları sağlanmıyorsa formu hata mesajıyla geri gösterir.
        if (!ModelState.IsValid)
        {
            model.HeroStats = await GetHeroStatsAsync();
            return View(model);
        }

        // API'ye sadece email bilgisini gönderiyoruz.
        var response = await _apiService.PostAsync<ForgotPasswordResponse>("auth/forgot-password", new { model.Email });

        // Başarısızsa hata mesajı ile formu geri gösterir.
        if (!response.Success)
        {
            ModelState.AddModelError(string.Empty, response.Message);
            model.HeroStats = await GetHeroStatsAsync();
            return View(model);
        }

        // API güvenlik için her durumda generic mesaj döner; aynı mesajı kullanıcıya gösteririz.
        model.IsSubmitted = true;
        model.Message = response.Data?.Message ?? "Eğer bu email sistemde kayıtlıysa şifre yenileme bağlantısı gönderildi.";
        model.HeroStats = await GetHeroStatsAsync();
        return View(model);
    }

    // Maildeki reset linkinden gelen yeni şifre belirleme sayfasını açar.
    [HttpGet]
    public async Task<IActionResult> ResetPassword(string? token)
    {
        // Giriş yapmış kullanıcı reset linki kullanmak yerine ana sayfaya yönlendirilir.
        if (HttpContext.Session.IsAuthenticated(_configuration))
            return RedirectToAction("Index", "Home");

        // Token yoksa formu açmayız; kullanıcıya kontrollü hata gösteririz.
        if (string.IsNullOrWhiteSpace(token))
        {
            return View(new ResetPasswordViewModel
            {
                IsInvalidLink = true,
                Message = "Şifre yenileme linki geçersiz veya süresi dolmuş.",
                HeroStats = await GetHeroStatsAsync()
            });
        }

        // Token hidden input ile POST'a taşınır.
        return View(new ResetPasswordViewModel
        {
            Token = token,
            HeroStats = await GetHeroStatsAsync()
        });
    }

    // Yeni şifre formu gönderildiğinde reset token ile API'de şifreyi günceller.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        // Validation kuralları sağlanmıyorsa formu hata mesajıyla geri gösterir.
        if (!ModelState.IsValid)
        {
            model.HeroStats = await GetHeroStatsAsync();
            return View(model);
        }

        // API'ye reset token ve yeni şifre bilgilerini gönderiyoruz.
        var resetData = new
        {
            model.Token,
            model.NewPassword,
            model.ConfirmNewPassword
        };

        var response = await _apiService.PostAsync<ResetPasswordResponse>("auth/reset-password", resetData);

        // Token geçersizse veya API hata dönerse aynı sayfada hata gösterir.
        if (!response.Success)
        {
            ModelState.AddModelError(string.Empty, response.Message);
            model.HeroStats = await GetHeroStatsAsync();
            return View(model);
        }

        // Başarılı reset sonrası kullanıcı yeni şifresiyle giriş yapabilsin diye Login'e yönlendirir.
        TempData["SuccessMessage"] = response.Data?.Message ?? "Şifreniz başarıyla yenilendi. Yeni şifrenizle giriş yapabilirsiniz.";
        return RedirectToAction("Login");
    }

    // Kayıt sayfasını açar.
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        // Zaten giriş yapmışsa ana sayfaya yönlendir.
        if (HttpContext.Session.IsAuthenticated(_configuration))
            return RedirectToAction("Index", "Home");

        // Form modeline hero istatistikleri de eklenir; sayilar API endpointinden gelir.
        return View(new RegisterViewModel
        {
            HeroStats = await GetHeroStatsAsync()
        });
    }

    // Kayıt formu gönderildiğinde çalışır.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Validation kuralları sağlanmıyorsa formu hata mesajıyla geri göster.
        if (!ModelState.IsValid)
        {
            model.HeroStats = await GetHeroStatsAsync();
            return View(model);
        }

        // API'ye göndermek için sadece backend'in beklediği alanları paketliyoruz.
        // ConfirmPassword UI için var, API'ye gönderilmez.
        var registerData = new
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = model.Password,
            Phone = model.Phone
        };

        // API'ye POST isteği gönderiyoruz.
        var response = await _apiService.PostAsync<RegisterResponse>("auth/register", registerData);

        // Başarısızsa hata mesajı ile formu geri gösterir.
        if (!response.Success)
        {
            ModelState.AddModelError(string.Empty, response.Message);
            model.HeroStats = await GetHeroStatsAsync();
            return View(model);
        }

        var data = response.Data!;

        // FAZ 1 davranışı: API token döndüyse otomatik giriş yapar.
        if (!string.IsNullOrEmpty(data.Token))
        {
            HttpContext.Session.SetString("JWToken", data.Token);
            HttpContext.Session.SetString("UserEmail", data.Email!);
            HttpContext.Session.SetString("UserFullName", data.FullName!);
            ClearRememberMeCookie();
            return RedirectToAction("Index", "Home");
        }

        // FAZ 2 davranışı: API token döndürmüyse email doğrulama gerekiyor.
        // Mesajı TempDataya koy, Login sayfasına yönlendir
        TempData["SuccessMessage"] = data.Message;
        return RedirectToAction("Login");
    }

    // Email dogrulama linkinden gelir
    // Maildeki link: /Auth/VerifyEmail?token=abc123
    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string? token)
    {
        // Kullanici zaten login ise (orn. ayni dogrulama linki ikinci kez aciliyor),
        // dogrulama akisi calistirmak yerine ana sayfaya yonlendiriyoruz.
        // Boylece sag ustte "Hosgeldin X" goruyorken "dogrulama basarisiz" celiskisi yasanmaz.
        if (HttpContext.Session.IsAuthenticated(_configuration))
            return RedirectToAction("Index", "Home");

        // Token yoksa direkt hata sayfasini goster.
        if (string.IsNullOrEmpty(token))
        {
            return View(new VerifyEmailViewModel
            {
                IsSuccess = false,
                Message = "Doğrulama linki eksik veya geçersiz."
            });
        }

        // API'ye token'i gonder dogrulama islmeinin sonucunu bekle
        // Basari donerse LoginResponse (token + mail + isim) alir.
        // Uri.EscapeDataString: token icindeki ozel karakterleri (+, =, &, /) URL-safe hale getirir.
        // Boylece query string parse hatalari ve parametre enjeksiyonu engellenir.
        var encodedToken = Uri.EscapeDataString(token);
        var response = await _apiService.GetAsync<LoginResponse>($"auth/verify-email?token={encodedToken}");

        // Basarisizsa (geceresiz token, suresi dolmus vb.) hata sayfasi gosterir.
        if (!response.Success)
        {
            return View(new VerifyEmailViewModel
            {
                IsSuccess = false,
                Message = response.Message
            });
        }

        // Basarili: API JWT token dondu, otomatik giris yapiyoruz.
        var data = response.Data!;
        HttpContext.Session.SetString("JWToken", data.Token);
        HttpContext.Session.SetString("UserEmail", data.Email);
        HttpContext.Session.SetString("UserFullName", data.FullName);
        ClearRememberMeCookie();

        // Basari ekrani - kullanici 'tamam anaysayfaya git' diye tiklayacak.
        return View(new VerifyEmailViewModel
        {
            IsSuccess = true,
            Message = "Email adresiniz başarıyla doğrulandı! Otomatik giriş yapıldı.",
            Email = data.Email
        });
    }

    // Resend formu gonderildiginde calisi.
    // Kullanici emal'ini girip "Tekrar Dogrulama Maili Gonder" tikladi.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendVerification(ResendVerificationViewModel model)
    {
        // Emmail validation kurallari saglanmiyorsa VerifyEmail sayfasini hata olarak goster.
        if (!ModelState.IsValid)
        {
            return View("VerifyEmail", new VerifyEmailViewModel
            {
                IsSuccess = false,
                Message = "Lütfen geçerli bir email adresi girin.",
                Email = model.Email
            });
        }

        // API'ye POST. Body: {Email}
        // Donus tipi onemli degil cunku Success/Message zaten ApiResponse<T>'de var.
        var response = await _apiService.PostAsync<object>("auth/resend-verification", new { Email = model.Email });

        // Basarisizsa (kullanici yok, mail gonderilemedi vs.) hata sayfasi.
        if (!response.Success)
        {
            return View("VerifyEmail", new VerifyEmailViewModel
            {
                IsSuccess = false,
                Message = response.Message,
                Email = model.Email
            });
        }

        // Basarili: Kullanici yesil mesaj gosterip Login'e yonlendir.
        TempData["SuccessMessage"] = "Doğrulama maili başarıyla gönderildi! Lütfen gelen kutunuzu kontrol edin.";
        return RedirectToAction("Login");
    }

    // Çıkış işlemini yapar.
    // Form POST ile gelir, CSRF token kontrolünden geçer.
    // Sessiondaki tüm auth bilgilerini siler ve anasayfaya yönlendirilir.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        ClearRememberMeCookie();
        return RedirectToAction("Index", "Home");
    }

    // Auth sayfalarinda gosterilecek otel ve sehir sayilarini API'deki hero-stats endpointinden alir.
    private async Task<HeroStatsViewModel?> GetHeroStatsAsync()
    {
        var response = await _apiService.GetAsync<HeroStatsViewModel>("hotels/hero-stats");
        return response.Success ? response.Data : null;
    }

    // Login cevabındaki JWT tokenı Beni Hatırla seçimine göre güvenli cookie'ye yazar.
    private void SetRememberMeCookie(LoginResponse data, bool rememberMe)
    {
        // Kullanıcı Beni Hatırla seçmediyse eski cookie kalmasın diye temizler.
        if (!rememberMe)
        {
            ClearRememberMeCookie();
            return;
        }

        // API'den gelen bitiş tarihini UTC olarak normalize eder.
        var expiresAt = data.ExpiresAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(data.ExpiresAt, DateTimeKind.Utc)
            : data.ExpiresAt.ToUniversalTime();

        // Token JavaScript tarafından okunamasın diye HttpOnly cookie olarak saklanır.
        Response.Cookies.Append(
            SessionAuthExtensions.RememberMeCookieName,
            data.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = new DateTimeOffset(expiresAt)
            });
    }

    // Logout, register auto-login ve doğrulama akışlarında eski Beni Hatırla cookie'sini temizler.
    private void ClearRememberMeCookie()
    {
        Response.Cookies.Delete(SessionAuthExtensions.RememberMeCookieName);
    }
}
