namespace HotelApp.Domain.DTOs;

// Admin panelindeki kullanici listesi ve kullanici durum islemleri icin donen DTO'dur.
public class AdminUserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
