namespace HotelApp.Web.Models;

// AdminLogsPageViewModel sınıfının amacı, admin log listeleme sayfasının filtre ve sonuç verisini taşımaktır.
// E44 filtreleri, sayfalama bilgileri ve log listesini tek modelde toplar.
// SOLID: SRP kullanır; sadece log listeleme sayfasının ekran durumunu temsil eder.
public class AdminLogsPageViewModel
{
    public string? Level { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Search { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalCount { get; set; }
    public List<AdminLogViewModel> Logs { get; set; } = new();
}
