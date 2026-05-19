using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

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
    public List<AdminHotelRoomViewModel> Rooms { get; set; } = new();
    public List<RoomAmenityViewModel> AllRoomAmenities { get; set; } = new();
}

public class HotelImageAdminFormViewModel
{
    [Required(ErrorMessage = "Görsel URL alanı zorunludur.")]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}

public class AdminAmenityItemViewModel : AmenityViewModel
{
    public bool IsAssigned { get; set; }
}

public class AdminAmenityPageViewModel
{
    public List<AmenityViewModel> Amenities { get; set; } = new();
    public AdminAmenityFormViewModel CreateForm { get; set; } = new();
    public AdminAmenityFormViewModel EditForm { get; set; } = new();
}

public class AdminAmenityFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Olanak adı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }
}

public class CreatedHotelResponseViewModel
{
    public int Id { get; set; }
}

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
}
