namespace HotelApp.Web.Helpers
{
    // Icon class adlarini tam CSS class formatina cevirir.
    // Birden fazla icon paketi (Bootstrap Icons, Material Design Icons, Tabler)
    // bir arada kullanildiginda her birinin font family prefix'i farklidir.
    //
    // Ornek donusler:
    //   "bi-water"      -> "bi bi-water"        (Bootstrap Icons)
    //   "mdi-pizza"     -> "mdi mdi-pizza"      (Material Design Icons)
    //   "ti-bed"        -> "ti ti-bed"          (Tabler Icons)
    //   "bi bi-water"   -> "bi bi-water"        (zaten tam)
    //   "water"         -> "bi bi-water"        (eski format, prefix yok)
    //   null / ""       -> fallback degeri
    public static class IconCssHelper
    {
        public static string Resolve(string? icon, string fallback = "bi bi-grid")
        {
            if (string.IsNullOrWhiteSpace(icon)) return fallback;

            var trimmed = icon.Trim();

            // Zaten tam class formatinda (font family prefix dahil)
            if (trimmed.StartsWith("bi ") || trimmed.StartsWith("mdi ") || trimmed.StartsWith("ti "))
                return trimmed;

            // Sadece ikon adi (icon paketi prefix'i var ama font family yok)
            if (trimmed.StartsWith("mdi-")) return $"mdi {trimmed}";
            if (trimmed.StartsWith("ti-")) return $"ti {trimmed}";
            if (trimmed.StartsWith("bi-")) return $"bi {trimmed}";

            // Hicbir prefix yok - varsayilan olarak Bootstrap Icons say (eski DB kayitlari)
            return $"bi bi-{trimmed}";
        }
    }
}
