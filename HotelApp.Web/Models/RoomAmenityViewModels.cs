using System.ComponentModel.DataAnnotations;

namespace HotelApp.Web.Models;

public class RoomAmenityViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
}

public class AdminRoomAmenityPageViewModel
{
    public List<RoomAmenityViewModel> RoomAmenities { get; set; } = new();
    public AdminRoomAmenityFormViewModel CreateForm { get; set; } = new();
    public AdminRoomAmenityFormViewModel EditForm { get; set; } = new();
}

public class AdminRoomAmenityFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Oda olanağı adı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }
}

public class AdminHotelRoomViewModel
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public int RoomTypeId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public decimal ExtraGuestPricePerNight { get; set; }
    public int MaxGuests { get; set; }
    public int TotalRooms { get; set; }
    public bool IsActive { get; set; }

    // Manzara bilgisi API'den gelir (NULL olabilir).
    public int? RoomViewId { get; set; }
    public string? RoomViewName { get; set; }
    public string? RoomViewIcon { get; set; }

    public List<RoomAmenityViewModel> RoomAmenities { get; set; } = new();
}
