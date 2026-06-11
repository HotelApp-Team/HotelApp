namespace HotelApp.Web.Models
{
    // Navbar katalog arama sayfasında kullanılan verileri taşır.
    // Arama ifadesini, sonuçları ve sayfalama bilgilerini içerir.
    public class CatalogSearchPageViewModel
    {
        // Kullanıcının navbar üzerinden yazdığı otel veya şehir bilgisidir.
        public string SearchTerm { get; set; } = string.Empty;

        // Aramayla eşleşen toplam otel sayısıdır.
        public int TotalCount { get; set; }

        // Kullanıcının görüntülediği mevcut sayfadır.
        public int CurrentPage { get; set; } = 1;

        // Bir sayfada gösterilecek otel sayısıdır.
        public int PageSize { get; set; } = 9;

        // Arama sonucunda gösterilecek otel kartlarıdır.
        public List<HotelCardViewModel> Hotels { get; set; } = new();
    }
}
