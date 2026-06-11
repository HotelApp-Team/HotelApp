using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

// Admin otel listeleme sayfasında kullanılan filtre, sıralama,
// sayfalama ve otel kartı listesini taşıyan view modeldir.
public class AdminHotelListPageViewModel
{
    public string? City { get; set; }
    public byte? StarRating { get; set; }
    public bool? IsActive { get; set; }
    public string SortBy { get; set; } = "name";
    public string SortOrder { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int TotalCount { get; set; }
    public List<HotelCardViewModel> Hotels { get; set; } = new();
}

// Admin otel oluşturma ve düzenleme formunda kullanılan ana view modeldir.
// Otelin temel bilgilerini, görsellerini, olanaklarını, oda tiplerini,
// oda manzaralarını ve mevcut oda listesini tek modelde taşır.
public class AdminHotelFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Otel adı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Adres zorunludur.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şehir bilgisi zorunludur.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ülke bilgisi zorunludur.")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string? Email { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    [Range(1, 5, ErrorMessage = "Yıldız puanı 1 ile 5 arasında olmalıdır.")]
    public byte StarRating { get; set; }

    public bool IsActive { get; set; } = true;

    public List<HotelImageViewModel> Images { get; set; } = new();
    public List<AdminAmenityItemViewModel> AllAmenities { get; set; } = new();
    public List<int> SelectedAmenityIds { get; set; } = new();
    public List<RoomTypeAdminViewModel> RoomTypes { get; set; } = new();
    public List<RoomViewAdminViewModel> RoomViews { get; set; } = new();
    public List<AdminHotelRoomViewModel> Rooms { get; set; } = new();
    public List<RoomAmenityViewModel> AllRoomAmenities { get; set; } = new();
}

// Admin panelinde otel veya oda görseli ekleme/güncelleme formlarında kullanılır.
// Görsel URL, ana görsel durumu ve ekrandaki sıralama bilgisini taşır.
public class HotelImageAdminFormViewModel
{
    [Required(ErrorMessage = "Görsel URL alanı zorunludur.")]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}

// Olanak listesindeki her bir olanağın otele atanıp atanmadığını tutar.
// Checkbox listesinde seçili olanakları göstermek için kullanılır.
public class AdminAmenityItemViewModel : AmenityViewModel
{
    public bool IsAssigned { get; set; }
}

// Admin olanak yönetimi sayfasında kullanılan view modeldir.
// Mevcut olanak listesini, oluşturma formunu ve düzenleme formunu taşır.
public class AdminAmenityPageViewModel
{
    public List<AmenityViewModel> Amenities { get; set; } = new();
    public AdminAmenityFormViewModel CreateForm { get; set; } = new();
    public AdminAmenityFormViewModel EditForm { get; set; } = new();
}

// Admin panelinde olanak oluşturma ve güncelleme formlarında kullanılır.
// Olanak adı zorunludur, ikon bilgisi isteğe bağlıdır.
public class AdminAmenityFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Olanak adı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }
}

// Otel oluşturulduktan sonra API'den dönen Id bilgisini taşır.
// Create işleminden sonra Edit sayfasına yönlendirmek için kullanılır.
public class CreatedHotelResponseViewModel
{
    public int Id { get; set; }
}

// Admin panelinde otele yeni oda ekleme formunda kullanılır.
// Oda tipi, fiyat, ek misafir ücreti, toplam oda sayısı,
// isteğe bağlı görsel ve manzara bilgisini taşır.
public class AdminHotelRoomFormViewModel
{
    [Required(ErrorMessage = "Oda tipi seçiniz.")]
    public int RoomTypeId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal PricePerNight { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ExtraGuestPricePerNight { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalRooms { get; set; }

    // Oda eklerken isteğe bağlı oda görseli eklemek için kullanılır.
    // Bu görsel RoomType'a değil, seçili otele ait HotelRoom kaydına bağlanır.
    public string? ImageUrl { get; set; }

    public bool IsMain { get; set; } = true;

    public int DisplayOrder { get; set; }

    // Manzara opsiyoneldir; admin bos birakirsa NULL kalir.
    public int? RoomViewId { get; set; }
}
