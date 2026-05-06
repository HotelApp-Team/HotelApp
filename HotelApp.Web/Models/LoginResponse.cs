namespace HotelApp.Web.Models;

// LoginResponse sınıfının amacı, API'den gelen başarılı giriş cevabını web tarafında taşımaktır.
// JWT token, email, tam ad ve token süresini session'a yazılabilecek şekilde tutar.
// SOLID: SRP kullanır; sadece web login cevap verisini temsil eder.

public class LoginResponse
{
    public string Token { get; set; } = null!; //JWT token
    public string Email { get; set; } = null!; //Kullanıcının email adresi
    public string FullName { get; set; } = null!; //Kullanıcının tam adı
    public DateTime ExpiresAt { get; set; } //Token'ın geçerlilik süresi
}
