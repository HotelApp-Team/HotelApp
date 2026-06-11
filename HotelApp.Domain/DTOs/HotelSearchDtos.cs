using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// Otel arama iþlemi için kullanýcýdan gelen filtreleri taþýr.
// Þehir, tarih aralýðý, yýldýz ve misafir sayýsý gibi arama kriterlerini içerir.
public class HotelSearchRequestDto
{
    [Required(ErrorMessage = "Þehir zorunludur.")]
    public string? City { get; set; }

    public byte? StarRating { get; set; }
    public decimal? MinRating { get; set; }

    [Required(ErrorMessage = "Giriþ tarihi zorunludur.")]
    public DateTime CheckIn { get; set; }

    [Required(ErrorMessage = "Çýkýþ tarihi zorunludur.")]
    public DateTime CheckOut { get; set; }

    [Required(ErrorMessage = "Misafir sayýsý zorunludur.")]
    [Range(1, 20, ErrorMessage = "Misafir sayýsý 1 ile 20 arasýnda olmalýdýr.")]
    public int GuestCount { get; set; }
}

// Otel arama sonucunda dönen liste bilgisini taþýr.
// TotalCount toplam sonucu, Hotels ise bulunan otelleri içerir.
public class HotelSearchResponseDto
{
    public int TotalCount { get; set; }
    public List<HotelListDto> Hotels { get; set; } = new();
}

// Sayfalý listeleme iþlemlerinde ortak cevap modeli olarak kullanýlýr.
// TotalCount, Page, PageSize ve Data alanlarýyla liste sonucunu taþýr.
public class PagedResponseDto<T>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Data { get; set; } = new();
}
