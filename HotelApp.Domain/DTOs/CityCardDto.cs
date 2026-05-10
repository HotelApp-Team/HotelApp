namespace HotelApp.Domain.DTOs;

// Anasayfa şehir kartlarında gösterilecek özet veriyi taşır.
public class CityCardDto
{
    public string Name { get; set; } = string.Empty;
    public int HotelCount { get; set; }
    public bool IsBig { get; set; }
}
