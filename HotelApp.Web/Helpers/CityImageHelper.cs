namespace HotelApp.Web.Helpers;

// CityImageHelper sınıfının amacı, web tarafındaki şehir kartları için sabit görsel yollarını üretmektir.
// Şehir görseli sunum katmanı detayı olduğu için Domain veya Service katmanına taşınmaz.
// SOLID: SRP kullanır; sadece şehir adına göre görsel yolu belirleme sorumluluğunu taşır.
public static class CityImageHelper
{
    private static readonly Dictionary<string, string> CityImageMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Istanbul"] = "istanbul.jpg",
        ["İstanbul"] = "istanbul.jpg",
        ["Ankara"] = "ankara.jpg",
        ["Antalya"] = "antalya.jpg",
        ["Izmir"] = "izmir.jpg",
        ["İzmir"] = "izmir.jpg",
        ["Bursa"] = "bursa.jpg",
        ["Aydin"] = "aydin.jpg",
        ["Aydın"] = "aydin.jpg",
        ["Gaziantep"] = "gaziantep.jpg",
        ["Antep"] = "gaziantep.jpg",
        ["Kayseri"] = "kayseri.jpg",
        ["Konya"] = "konya.jpg",
        ["Trabzon"] = "trabzon.jpg",
        ["Fethiye"] = "fethiye.jpg",
        ["Bodrum"] = "bodrum.jpg",
        ["Mugla"] = "mugla.jpg",
        ["Muğla"] = "mugla.jpg",
        ["Canakkale"] = "çanakkale.jpg",
        ["Çanakkale"] = "çanakkale.jpg"
    };

    // Şehir adına göre wwwroot altındaki sabit görsel yolunu döndürür.
    public static string GetImageUrl(string? cityName)
    {
        // Şehir adı boşsa varsayılan şehir görseli kullanılır.
        if (string.IsNullOrWhiteSpace(cityName))
            return "/images/cities/default-city.svg";

        return CityImageMap.TryGetValue(cityName.Trim(), out var fileName)
            ? $"/images/cities/{fileName}"
            : "/images/cities/default-city.svg";
    }
}
