namespace HotelApp.Web.Models;

// Otel arama API'sinden dönen sonucu web tarafında taşır.
// Toplam kayıt sayısı ve bulunan otel kartlarını içerir.
public class HotelSearchResponseViewModel
{
    public int TotalCount { get; set; }
    public List<HotelCardViewModel> Hotels { get; set; } = new();
}

// Otel arama sayfasında kullanılan view modeldir.
// Kullanıcının seçtiği filtreleri, sayfalama bilgisini ve arama sonucundaki otelleri taşır.
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
