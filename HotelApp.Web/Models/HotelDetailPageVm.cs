namespace HotelApp.Web.Models;

public class HotelDetailPageVm
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal? AvgRating { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public byte StarRating { get; set; }
    public bool IsActive { get; set; }
    public List<AmenityViewModel> Amenities { get; set; } = new();
    public List<HotelImageViewModel> Images { get; set; } = new();
    public List<AvailableRoomViewModel> Rooms { get; set; } = new();
    public List<ReviewViewModel> Reviews { get; set; } = new();

    public CreateReservationViewModel BookingForm { get; set; } = new();

    public bool IsAuthenticated { get; set; }
}

public class HotelImageViewModel
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}

public class AmenityViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
}
