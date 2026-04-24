namespace HotelApp.Domain.DTOs;

// Search Request DTO
public class HotelSearchRequestDto
{
    public string? City { get; set; }
    public byte? StarRating { get; set; }
    public decimal? MinRating { get; set; }

    // Availability search icin (ileride lazim olacak)
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public int? GuestCount { get; set; }
}

// Search Response DTO
public class HotelSearchResponseDto
{
    public int TotalCount { get; set; }
    public List<HotelListDto> Hotels { get; set; } = new();
}

// Genel sayfalama cevabi - GetAllFilteredAsync gibi icin
public class PagedResponseDto<T>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Data { get; set; } = new();
}
