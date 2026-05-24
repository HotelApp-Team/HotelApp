using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApiService _apiService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApiService apiService, ILogger<HomeController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var citiesResponse = await _apiService.GetAsync<List<string>>("hotels/cities");
        var heroStatsResponse = await _apiService.GetAsync<HeroStatsViewModel>("hotels/hero-stats");
        var hotelsResponse = await _apiService.GetAsync<PagedHotelListResponse>("hotels?page=1&pageSize=6");
        var featuredCitiesResponse = await _apiService.GetAsync<List<CityCardViewModel>>("hotels/featured-cities?count=5");
        var reviewsResponse = await _apiService.GetAsync<List<ReviewViewModel>>("reviews/featured?count=3");

        // API'den gelen vitrin verilerini anasayfa modeline toplar.
        var model = new HomePageViewModel
        {
            Cities = citiesResponse.Success ? citiesResponse.Data ?? new List<string>() : new List<string>(),
            HeroStats = heroStatsResponse.Success ? heroStatsResponse.Data : null,
            FeaturedHotels = hotelsResponse.Success ? hotelsResponse.Data?.Data ?? new List<HotelCardViewModel>() : new List<HotelCardViewModel>(),
            FeaturedCities = featuredCitiesResponse.Success ? featuredCitiesResponse.Data ?? new List<CityCardViewModel>() : new List<CityCardViewModel>(),
            FeaturedReviews = reviewsResponse.Success ? reviewsResponse.Data ?? new List<ReviewViewModel>() : new List<ReviewViewModel>()
        };

        return View(model);

    }
    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Faq() => View();

    // Status code (404, 403, 500 vb.) yakalandiginda Program.cs'teki
    // UseStatusCodePagesWithReExecute("/Home/Error/{0}") tarafindan cagrilir.
    // Tarayicinin default 404 ekrani yerine siteyle uyumlu nazik bir sayfa gosterir.
    [Route("Home/Error/{code:int?}")]
    public IActionResult Error(int? code)
    {
        var model = new ErrorPageViewModel
        {
            StatusCode = code ?? 500,
            Title = (code ?? 500) switch
            {
                404 => "Sayfa bulunamadı",
                403 => "Erişim izniniz yok",
                401 => "Giriş yapmanız gerekiyor",
                _   => "Bir şeyler ters gitti"
            },
            Message = (code ?? 500) switch
            {
                404 => "Aradığınız sayfa burada değil. Adres yanlış olabilir ya da sayfa kaldırılmış olabilir.",
                403 => "Bu sayfaya erişim yetkiniz yok. Hesabınızla yetkili olduğunuz alanlara dönebilirsiniz.",
                401 => "Bu sayfayı görmek için giriş yapmanız gerekiyor.",
                _   => "Beklenmeyen bir hata oluştu. Birkaç saniye sonra tekrar deneyebilirsiniz."
            }
        };

        return View("Error", model);
    }
}
