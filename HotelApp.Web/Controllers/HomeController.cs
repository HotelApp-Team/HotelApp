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
        var hotelsResponse = await _apiService.GetAsync<PagedHotelListResponse>("hotels?page=1&pageSize=6");

        var model = new HomePageViewModel
        {
            Cities = citiesResponse.Success ? citiesResponse.Data ?? new List<string>() : new List<string>(),
            FeaturedHotels = hotelsResponse.Success ? hotelsResponse.Data?.Data ?? new List<HotelCardViewModel>() : new List<HotelCardViewModel>()
        };

        return View(model);
    }
}
