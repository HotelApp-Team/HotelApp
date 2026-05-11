using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// AdminUserMgmtController sınıfının amacı, admin kullanıcı yönetimi sayfasını yönetmektir.
// E37 kullanıcı listeleme, E38 aktif/pasif ve E39 rol güncelleme akışlarını MVC tarafında API'ye bağlar.
// SOLID: SRP kullanır; sadece admin kullanıcı yönetimi sayfa akışından sorumludur.

[Route("admin/users")]
[ServiceFilter(typeof(AdminFilter))]
[ApiExplorerSettings(IgnoreApi = true)]
public class AdminUserMgmtController : Controller
{
    private readonly ApiService _apiService;

    public AdminUserMgmtController(ApiService apiService)
    {
        _apiService = apiService;
    }

    // E37 için kullanıcı listesini arama, aktiflik ve sayfalama filtreleriyle getirir.
    [HttpGet]
    public async Task<IActionResult> Index(string? search, bool? isActive, int page = 1, int pageSize = 10)
    {
        // View'dan gelen filtreleri API'nin beklediği query parametrelerine mapler.
        var queryParams = new Dictionary<string, string?>
        {
            ["search"] = search,
            ["isActive"] = isActive?.ToString().ToLower(),
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        };

        // Boş olanları atar, kalanları key=value formatına çevirip query string oluşturur.
        var query = "admin/users?" +
            string.Join("&",
            queryParams
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));

        var response = await _apiService.GetAsync<AdminUsersResponseViewModel>(query);

        // Filtre değerlerini tekrar view model'e koyar; sayfa yenilenince seçimler kaybolmaz.
        var model = new AdminUsersPageViewModel
        {
            Search = search,
            IsActive = isActive,
            CurrentPage = page,
            PageSize = pageSize,
        };

        // API çağrısı başarısızsa boş listeyle sayfayı açar ve hata mesajını gösterir.
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return View(model);
        }

        // API'den gelen sayfalı kullanıcı sonucunu ve özet sayaçları view model'e mapler.
        model.TotalCount = response.Data?.TotalCount ?? 0;
        model.ActiveCount = response.Data?.ActiveCount ?? 0;
        model.PassiveCount = response.Data?.PassiveCount ?? 0;
        model.AdminCount = response.Data?.AdminCount ?? 0;
        model.Users = response.Data?.Data ?? new();

        return View(model);
    }

    // E38 için seçili kullanıcının aktif/pasif durumunu günceller.
    [HttpPost("{id}/status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, bool isActive)
    {
        // Formdan gelen aktiflik bilgisini API'nin beklediği request body yapısına mapler.
        var body = new { isActive = isActive };

        var response = await _apiService.PutAsync<AdminUserViewModel>($"admin/users/{id}/status",
            body);

        // İşlem sonucuna göre kullanıcıya başarı veya hata mesajı gösterir.
        if (response.Success)
            TempData["Success"] = "Kullanıcı durumu güncellendi.";
        else
            TempData["Error"] = response.Message;

        return RedirectToAction(nameof(Index));
    }

    // E39 için seçili kullanıcının rollerini günceller.
    [HttpPost("{id}/roles")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRoles(int id, List<string> roles)
    {
        // Formdan gelen rol listesini AdminUpdateUserRolesDto ile uyumlu request body'ye mapler.
        var body = new { roles = roles };

        var response = await _apiService.PutAsync<AdminUserViewModel>($"admin/users/{id}/roles",
            body);

        // İşlem sonucuna göre kullanıcıya başarı veya hata mesajı gösterir.
        if (response.Success)
            TempData["Success"] = "Kullanıcı rolleri güncellendi.";
        else
            TempData["Error"] = response.Message;

        return RedirectToAction(nameof(Index));
    }
}
