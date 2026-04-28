using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// Giris, kayit ve email dogrulama sayfalarini yoneten controller.
public class AuthController : Controller
{
    // API ile konuşmak için ApiService'i kullanacağız.
    // DI ile Program.cs'te kaydedildiği için otomatik olarak sağlanır.
    private readonly ApiService _apiService;

    public AuthController(ApiService apiService)
    {
        _apiService = apiService;
    }

    // Giriş sayfasını açar.
    // Kullanıcı korumalı sayfalara erişmeye çalıştığında buraya yönlendirilir.
    // Sistem onu Login'e yonlendirirken URL'ye ?returnUrl=/Reservations/My ekler.
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Zaten giriş yapmışsa: ReturnUrl varsa oraya (sadece site içi adresse), yoksa ana sayfaya yönlendir.
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
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

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // Login formu gönderildiğinde çalışır.
    // [ValidateAntiForgeryToken] form sahteciliğine karşı koruma sağlar.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Validation kuralları sağlanmıyorsa formu hata mesajıyla geri gösterir.
        if (!ModelState.IsValid)
            return View(model);

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
            return View(model);
        }

        // Başarılı API'nin döndürdüğü token ve kullanıcı bilgilerini sessiona yaz.
        // API service her istekte Session'dan token'ı okuyarak Authorization header'ına ekleyecek.
        var data = response.Data!;
        HttpContext.Session.SetString("JWToken", data.Token);
        HttpContext.Session.SetString("UserEmail", data.Email);
        HttpContext.Session.SetString("UserFullName", data.FullName);

        // ReturnUrl varsa oraya yönlendir, yoksa ana sayfaya.
        // Url.IsLocalUrl ile güvenli bir yönlendirme yapıyoruz, dışarıya yönlendirmiyoruz.
        // kotu niyetli biri ?returnUrl=https://kotu-site.com gonderirse engellenir.
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    // Kayıt sayfasını açar.
    [HttpGet]
    public IActionResult Register()
    {
        // Zaten giriş yapmışsa ana sayfaya yönlendir.
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            return RedirectToAction("Index", "Home");

        return View(new RegisterViewModel());
    }

    // Kayıt formu gönderildiğinde çalışır.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Validation kuralları sağlanmıyorsa formu hata mesajıyla geri göster.
        if (!ModelState.IsValid)
            return View(model);

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
            return View(model);
        }

        var data = response.Data!;

        // FAZ 1 davranışı: API token döndüyse otomatik giriş yapar.
        if (!string.IsNullOrEmpty(data.Token))
        {
            HttpContext.Session.SetString("JWToken", data.Token);
            HttpContext.Session.SetString("UserEmail", data.Email!);
            HttpContext.Session.SetString("UserFullName", data.FullName!);
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
        // Token yoksa direkt hata sayfasini goster.
        if (string.IsNullOrEmpty(token))
        {
            return View(new VerifyEmailViewModel
            {
                IsSuccess = false,
                Message = "Dogrulama linki eksik veya geçersiz."
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
        TempData["SuccessMessage"] = "Dogrulama maili başarıyla gönderildi! Lütfen gelen kutunuzu kontrol edin.";
        return RedirectToAction("Login");
    }
}
