namespace HotelApp.Web.Models;

// Ana sayfada gösterilecek tüm verileri tek modelde taşır.
// Şehir listesi, hero istatistikleri, öne çıkan oteller,
// öne çıkan şehirler ve yorumlar bu model üzerinden view'a gönderilir.
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

// Otel kartlarında gösterilecek özet otel bilgilerini taşır.
// Ana sayfa, arama sonucu ve admin liste ekranlarında kullanılabilir.
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
    public bool IsActive { get; set; }
}

// Sayfalı otel listeleme API cevabını web tarafında karşılamak için kullanılır.
// Toplam kayıt sayısı, sayfa bilgisi ve otel kartı listesini taşır.
public class PagedHotelListResponse
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<HotelCardViewModel> Data { get; set; } = new();
}
