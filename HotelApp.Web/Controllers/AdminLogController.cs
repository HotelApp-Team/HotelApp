using HotelApp.Web.Filters;
using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Web.Controllers;

// AdminLogController sınıfının amacı, admin sistem logları sayfasını yönetmektir.
// E44 log listeleme endpoint'ini filtre ve sayfalama parametreleriyle çağırır, log detayını view içinde güvenli gösterir.
// SOLID: SRP kullanır; sadece admin log listeleme sayfa akışından sorumludur.

[Route("admin/log")]
[ServiceFilter(typeof(AdminFilter))]
[ApiExplorerSettings(IgnoreApi = true)]
public class AdminLogController : Controller
{
    private readonly ApiService _apiService;

    public AdminLogController(ApiService apiService)
    {
        _apiService = apiService;
    }

    // E44 için log listesini filtre ve sayfalama bilgileriyle hazırlar.
    [HttpGet]
    public async Task<IActionResult> Index(
        string? level,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? search,
        int page = 1,
        int pageSize = 20
        )
    {
        // View'dan gelen filtreleri API'nin beklediği query parametrelerine mapler.
        var queryParams = new Dictionary<string, string?>
        {
            ["level"] = level,
            ["dateFrom"] = dateFrom?.ToString("yyyy-MM-dd"),
            ["dateTo"] = dateTo?.ToString("yyyy-MM-dd"),
            ["search"] = search,
            ["pageNumber"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        };

        // Boş filtreleri query string'e eklemez; böylece API'ye temiz URL gönderilir.
        var query = "admin/logs?" +
            string.Join("&",
                queryParams
                    .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                    .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));

        var response = await _apiService.GetAsync<PagedResponseViewModel<AdminLogViewModel>>(query);

        // Filtre değerlerini tekrar view model'e koyar; hata olsa bile form seçili değerleri korur.
        var model = new AdminLogsPageViewModel
        {
            Level = level,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Search = search,
            CurrentPage = page,
            PageSize = pageSize
        };

        // API çağrısı başarısızsa boş listeyle sayfayı açar ve hata mesajını gösterir.
        if (!response.Success)
        {
            TempData["Error"] = response.Message;
            return View(model);
        }

        // API'den gelen sayfalı sonucu view model'e mapler.
        model.TotalCount = response.Data?.TotalCount ?? 0;
        model.Logs = response.Data?.Data ?? new();

        return View(model);
    }
}
