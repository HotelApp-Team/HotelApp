namespace HotelApp.Web.Models;

// UserResponse sınıfının amacı, API'den dönen kullanıcı profil cevabını web tarafında karşılamaktır.
// Profil ekranında gösterilecek kimlik ve iletişim bilgilerini taşır.
// SOLID: SRP kullanır; sadece kullanıcı cevap verisini temsil eder.
public class UserResponse
{
    // Kullanıcının sistemdeki benzersiz kimlik bilgisini taşır.
    public int Id { get; set; }
    // Kullanıcının profil ekranında gösterilecek ad bilgisini taşır.
    public string FirstName { get; set; } = null!;
    // Kullanıcının profil ekranında gösterilecek soyad bilgisini taşır.
    public string LastName { get; set; } = null!;
    // Kullanıcının hesap e-posta bilgisini taşır.
    public string Email { get; set; } = null!;
    // Kullanıcının iletişim telefon bilgisini taşır.
    public string Phone { get; set; } = null!;
    // Kullanıcının hesap oluşturma tarihini taşır.
    public DateTime CreatedAt { get; set; }
}
