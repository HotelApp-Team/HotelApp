namespace HotelApp.Web.Models
{
    // Admin Olanak Yonetimi sayfasi icin wrapper ViewModel.
    // Tek sayfada hem Otel Olanaklari hem Oda Olanaklari listeleri bulunur.
    // Form post'lari mevcut AmenityController ve RoomAmenityController action'larina
    // gitmeye devam eder; bu VM sadece sayfayi render etmek icin kullanilir.
    public class AdminAmenityMgmtPageViewModel
    {
        public List<AmenityViewModel> Amenities { get; set; } = new();
        public List<RoomAmenityViewModel> RoomAmenities { get; set; } = new();
    }
}
