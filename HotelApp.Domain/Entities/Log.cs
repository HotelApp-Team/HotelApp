using System;
using System.Collections.Generic;

namespace HotelApp.Domain.Entities;

// Log sınıfının amacı, Logs tablosundaki sistem kayıtlarını temsil etmektir.
// Hangi işlemde ne olduğu, işlemi kimin yaptığı ve hata detayı gibi log bilgilerini tutar.
// SOLID: SRP kullanır; sadece log entity verisini temsil eder.

public partial class Log
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

    public virtual User? User { get; set; }
}
