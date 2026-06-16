namespace HotelApp.Domain.DTOs;

// Admin otel listesi filtrelerinde kullanilacak secenekleri tasir.
// Sehir ve otel dropdownlarini tek istekte beslemek icin kullanilir.
public class HotelFilterOptionsDto
{
    // Sistemdeki (aktif + pasif) otellerin bulundugu tekil sehir adlari.
    public List<string> Cities { get; set; } = new();

    // Otel dropdown'i icin gerekli ozet otel bilgileri.
    public List<HotelFilterOptionItemDto> Hotels { get; set; } = new();
}

// Otel dropdown'inda gosterilecek tek bir otelin ozet bilgisi.
// City alani, secilen sehre gore client tarafinda daraltma yapmak icindir.
public class HotelFilterOptionItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
