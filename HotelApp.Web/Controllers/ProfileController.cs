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

    [HttpGet]
    // E03 icin kullanıcının profil bilgilerini alır ve Profile/Index ekranını hazırlar.
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
        var model = BuildPage(profileViewModel);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // E46 icin profil güncelleme formundan gelen bilgileri API'ye gönderir.
    public async Task<IActionResult> UpdateProfile([Bind(Prefix = "Profile")] ProfileViewModel model)
    {
        // Kullanıcı giriş yapmış mı kontrol eder.
        if (!HttpContext.Session.IsAuthenticated())
            return RedirectToAction("Login", "Auth");

        // Form validasyonu başarısızsa aynı sayfayı hata mesajlarıyla tekrar gösterir.
        if (!ModelState.IsValid)
        {
            var invalidPage = BuildPage(model);
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
            TempData["SuccessMessage"] = "Profil bilgileriniz güncellendi.";
            return RedirectToAction("Index");
        }

        // API hata döndürürse mesajı form üzerinde gösterir.
        ModelState.AddModelError(string.Empty, response.Message);
        var page = BuildPage(model);
        return View("Index", page);
    }

    // Profil ve şifre değiştirme formlarını aynı View içinde kullanılacak sayfa modeline dönüştürür.
    private ProfilePageViewModel BuildPage(ProfileViewModel profile) =>
        new ProfilePageViewModel
        {
            Profile = profile,
            ChangePassword = new ChangePasswordViewModel()
        };
}