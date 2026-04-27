namespace HotelApp.Web.Models;

//API'den dönen başarılı giriş yanıtını taşır
//Backend'in LoginResponseDto'su ile aynı alanları içerir.
public class LoginResponse
{
    public string Token { get; set; } = null!; //JWT token
    public string Email { get; set; } = null!; //Kullanıcının email adresi
    public string FullName { get; set; } = null!; //Kullanıcının tam adı
    public DateTime ExpiresAt { get; set; } //Token'ın geçerlilik süresi
}
