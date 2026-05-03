namespace HotelApp.Domain.DTOs;

// E39 için adminin kullanıcının rol listesini güncelleme isteğini taşır.
public class AdminUpdateUserRolesDto
{
    public List<string> Roles { get; set; } = new();
}
