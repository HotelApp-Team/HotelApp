namespace HotelApp.Web.Models;

// ApiErrorResponse sınıfının amacı, API'den gelen standart hata cevabını okumaktır.
// Başarı durumu ve hata mesajını web tarafında anlaşılır şekilde taşır.
// SOLID: SRP kullanır; sadece API hata cevap verisini temsil eder.

public class ApiErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
