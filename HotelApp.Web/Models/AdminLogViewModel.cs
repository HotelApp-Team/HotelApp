namespace HotelApp.Web.Models;

// AdminLogViewModel sınıfının amacı, E44/E45 sistem logu verisini admin log ekranına taşımaktır.
// Log seviyesi, mesaj, kullanıcı bilgisi, controller/action ve hata detayı alanlarını view için hazır tutar.
// SOLID: SRP kullanır; sadece tek bir log kaydının görüntüleme verisini temsil eder.
public class AdminLogViewModel
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;

    public int? UserId { get; set; }

    public string? UserEmail { get; set; }

    public string? Controller { get; set; }

    public string? Action { get; set; }

    public string? IpAddress { get; set; }

    public string? ExceptionDetail { get; set; }
}
