namespace HotelApp.Domain.DTOs;

// LogDto sınıfının amacı, log kayıtlarını API cevabı olarak taşımaktır.
// Log seviyesi, mesaj, kullanıcı, controller, action, IP ve hata detayını dışarıya düzenli şekilde aktarır.
// SOLID: SRP kullanır; sadece log cevap verisini temsil eder.

public class LogDto
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
