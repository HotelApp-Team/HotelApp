namespace HotelApp.Web.Models;

public class HotelSearchResponseViewModel
{
    public int TotalCount { get; set; }
    public List<HotelCardViewModel> Hotels { get; set; } = new();
}

public class HotelSearchPageViewModel
{
    public string? City { get; set; }
    public byte? StarRating { get; set; }
    public decimal? MinRating { get; set; }

    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public int? GuestCount { get; set; }

    public string? SortBy { get; set; }

    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public List<HotelCardViewModel> Hotels { get; set; } = new();
}
