namespace HotelApp.Domain.DTOs;

// AdminUpdateUserRolesDto sınıfının amacı, kullanıcının rol güncelleme isteğini taşımaktır.
// Admin panelinden gelen yeni rol listesini API tarafına aktarır.
// SOLID: SRP kullanır; sadece rol güncelleme istek verisini temsil eder.

public class AdminUpdateUserRolesDto
{
    public List<string> Roles { get; set; } = new();
}
