using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// E38 icin adminin kullanici aktiflik durumunu guncelleme istegini tasir.
public class AdminUpdateUserStatusDto
{
    // true aktif, false pasif anlamina gelir.
    // bool? kullanma sebebi: Body'de IsActive hic gelmezse default false sayilmasin, validation 400 dondursun.
    [Required(ErrorMessage = "IsActive alanı zorunludur.")]
    public bool? IsActive { get; set; }
}
