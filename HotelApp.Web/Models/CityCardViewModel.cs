namespace HotelApp.Web.Models
{
    /// <summary>
    /// Anasayfa "One Cikan Sehirler" bolumu icin kart modeli.
    /// Backend endpoint'inden donulecek: GET /api/hotels/featured-cities
    /// </summary>
    public class CityCardViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int HotelCount { get; set; }
        public bool IsBig { get; set; }
    }
}
