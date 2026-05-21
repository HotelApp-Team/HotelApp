using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

public class RoomAmenityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
}

public class RoomAmenityCreateDto
{
    [Required(ErrorMessage = "Oda olanağı adı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }
}

public class RoomAmenityUpdateDto
{
    [Required(ErrorMessage = "Id zorunludur.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Oda olanağı adı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }
}

public class HotelRoomAmenityAssignDto
{
    [Required(ErrorMessage = "RoomAmenityIds listesi zorunludur.")]
    public List<int> RoomAmenityIds { get; set; } = new();
}
