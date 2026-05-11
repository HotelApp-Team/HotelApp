namespace HotelApp.Web.Models;

public class HomePageViewModel
{
    public List<string> Cities { get; set; } = new();
    public HeroStatsViewModel? HeroStats { get; set; }
    public List<HotelCardViewModel> FeaturedHotels { get; set; } = new();
    public List<CityCardViewModel> FeaturedCities { get; set; } = new();
    public List<ReviewViewModel> FeaturedReviews { get; set; } = new();
}

// HeroStatsViewModel sınıfının amacı, anasayfa hero sayaçlarını view tarafına taşımaktır.
// SOLID: SRP kullanır; sadece hero istatistiklerinin sunum verisinden sorumludur.
public class HeroStatsViewModel
{
    public int HotelCount { get; set; }
    public int CityCount { get; set; }
}

public class HotelCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public byte StarRating { get; set; }
    public decimal? AvgRating { get; set; }
    public string? MainImageUrl { get; set; }

    public decimal? MinPrice { get; set; }
}

public class PagedHotelListResponse
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<HotelCardViewModel> Data { get; set; } = new();
}
