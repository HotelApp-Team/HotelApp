using System.ComponentModel.DataAnnotations;

namespace HotelApp.Domain.DTOs;

// AdminUpdateUserStatusDto sınıfının amacı, kullanıcının aktiflik durumunu güncelleme isteğini taşımaktır.
// Admin panelinden gelen aktif/pasif bilgisini validation kurallarıyla birlikte API tarafına aktarır.
// SOLID: SRP kullanır; sadece kullanıcı durum güncelleme istek verisini temsil eder.

public class AdminUpdateUserStatusDto
{
    // true aktif, false pasif anlamina gelir.
    // bool? kullanma sebebi: Body'de IsActive hic gelmezse default false sayilmasin, validation 400 dondursun.
    [Required(ErrorMessage = "IsActive alanı zorunludur.")]
    public bool? IsActive { get; set; }
}
