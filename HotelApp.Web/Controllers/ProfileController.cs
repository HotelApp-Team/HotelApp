using HotelApp.Web.Helpers;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// ProfileController sınıfının amacı, web tarafındaki kullanıcı profil ekranını yönetmektir.
// Profil görüntüleme, profil güncelleme ve şifre değiştirme formu için sayfa modelini API ile konuşturur.
// SOLID: SRP kullanır; sadece profil sayfası akışlarından sorumludur.
public class ProfileController : Controller
{
    private readonly ApiService _apiService;

    // Controller'ın API ile konuşması için gerekli ApiService bağımlılığını alır.
    public ProfileController(ApiService apiService)
    {
        _apiService = apiService;
    }

    // E03 için kullanıcının profil bilgilerini alır ve Profile/Index ekranını hazırlar.
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Kullanıcı giriş yapmış mı kontrol eder.
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Auth");

        // API'den giriş yapan kullanıcının profil bilgilerini çeker.
        var response = await _apiService.GetAsync<UserResponse>("auth/profile");

        // API cevabı başarısızsa veya veri boşsa kullanıcıyı ana sayfaya yönlendirir.
        if (!response.Success || response.Data == null)
        {
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Index", "Home");
        }

        // API'den gelen kullanıcı verisini profil formunda kullanılacak ViewModel'e mapler.
        var user = response.Data; // Null kontrolü yapıldığı için güvenle erişilebilir.
        var profileViewModel = new ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone
        };

        // Profil ve şifre değiştirme formlarını tek sayfa modelinde toplar.
        var model = await BuildPageAsync(profile: profileViewModel);
        if (model == null)
            return RedirectToAction("Index", "Home");

        return View(model);
    }

    // E46 için profil güncelleme formundan gelen bilgileri API'ye gönderir.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile([Bind(Prefix = "Profile")] ProfileViewModel model)
    {
        // Kullanıcı giriş yapmış mı kontrol eder.
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Auth");

        // Form validasyonu başarısızsa aynı sayfayı hata mesajlarıyla tekrar gösterir.
        if (!ModelState.IsValid)
        {
            var invalidPage = await BuildPageAsync(profile: model);
            if (invalidPage == null)
                return RedirectToAction("Index", "Home");

            return View("Index", invalidPage);
        }

        // Profil formundan gelen veriyi API'ye gönderilecek request nesnesine mapler.
        var data = new
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Phone = model.Phone
        };

        // Güncellenen profil bilgisini E46 API endpoint'ine gönderir.
        var response = await _apiService.PutAsync<UserResponse>("auth/profile", data);

        // Güncelleme başarılıysa kullanıcıya bilgi verip profil sayfasını yeniler.
        if (response.Success)
        {
            // Navbar'da görünen kullanıcı adını güncel profil bilgisiyle senkronlar.
            HttpContext.Session.SetString("UserFullName", $"{model.FirstName} {model.LastName}");
            TempData["SuccessMessage"] = "Profil bilgileriniz güncellendi.";
            return RedirectToAction("Index");
        }

        // API hata döndürürse mesajı form üzerinde gösterir.
        ViewData["ProfileErrorMessage"] = response.Message;
        var page = await BuildPageAsync(profile: model);
        if (page == null)
            return RedirectToAction("Index", "Home");

        return View("Index", page);
    }

    // E47 için şifre değiştirme formundan gelen bilgileri API'ye gönderir.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([Bind(Prefix = "ChangePassword")] ChangePasswordViewModel model)
    {
        // Kullanıcı giriş yapmış mı kontrol eder.
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Auth");

        // Şifre formu validasyonu başarısızsa aynı sayfayı hata mesajlarıyla tekrar gösterir.
        if (!ModelState.IsValid)
        {
            var page = await BuildPageAsync(changePassword: model);
            if (page == null)
                return RedirectToAction("Index", "Home");

            return View("Index", page);
        }

        // Formdan gelen şifre bilgilerini E47 API endpoint'ine gönderir.
        var response = await _apiService.PutAsync("auth/change-password", model);

        // Şifre değişimi başarılıysa kullanıcıya bilgi verip profil sayfasını yeniler.
        if (response.Success)
        {
            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
            return RedirectToAction("Index");
        }

        // API hata döndürürse mesajı form üzerinde gösterir.
        ViewData["PasswordErrorMessage"] = response.Message;
        var errorPage = await BuildPageAsync(changePassword: model);
        if (errorPage == null)
            return RedirectToAction("Index", "Home");

        return View("Index", errorPage);
    }

    // E03, E46 ve E47 akışlarında profil ve şifre formlarını aynı View için sayfa modeline dönüştürür.
    private async Task<ProfilePageViewModel?> BuildPageAsync(
        ProfileViewModel? profile = null,
        ChangePasswordViewModel? changePassword = null)
    {
        // Profil modeli verilmediyse güncel profil bilgisi API'den çekilir.
        if (profile == null)
        {
            var response = await _apiService.GetAsync<UserResponse>("auth/profile");

            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = string.IsNullOrWhiteSpace(response.Message)
                    ? "Profil bilgileri alınamadı."
                    : response.Message;

                return null;
            }

            // API'den gelen profil cevabını forma basılacak ViewModel'e mapler.
            profile = new ProfileViewModel
            {
                FirstName = response.Data.FirstName,
                LastName = response.Data.LastName,
                Email = response.Data.Email,
                Phone = response.Data.Phone
            };
        }

        // Profil ve şifre formlarını tek sayfa modelinde birleştirir.
        return new ProfilePageViewModel
        {
            Profile = profile,
            ChangePassword = changePassword ?? new ChangePasswordViewModel()
        };
    }
}
