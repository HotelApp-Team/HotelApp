namespace HotelApp.Domain.DTOs;

// HeroStatsDto sınıfının amacı, anasayfa hero alanında gösterilecek temel sayaçları taşımaktır.
// SOLID: SRP kullanır; sadece hero istatistik response verisinden sorumludur.
public class HeroStatsDto
{
    public int HotelCount { get; set; }
    public int CityCount { get; set; }
}
