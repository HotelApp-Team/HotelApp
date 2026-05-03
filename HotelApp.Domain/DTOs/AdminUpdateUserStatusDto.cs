namespace HotelApp.Domain.DTOs;

// E38 icin adminin kullanici aktiflik durumunu guncelleme istegini tasir.
public class AdminUpdateUserStatusDto
{
    // true aktif, false pasif anlamina gelir.
    public bool IsActive { get; set; }
}
