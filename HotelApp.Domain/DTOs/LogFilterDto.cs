namespace HotelApp.Domain.DTOs;

// LogFilterDto sınıfının amacı, admin log listesi sorgusu için filtreleri tek nesnede taşımaktır.
// E44 endpoint'i bu DTO'yu query string'den alıp LoggerService'e iletir.
// SOLID: SRP kullanır; sadece log filtre verisini temsil eder.
public class LogFilterDto
{
    public string? Level { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
