namespace HotelApp.Web.Models;

public class HomePageViewModel
{
    public List<string> Cities { get; set; } = new();
    public List<HotelCardViewModel> FeaturedHotels { get; set; } = new();
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
