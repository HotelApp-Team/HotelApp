namespace HotelApp.Domain.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }

}
