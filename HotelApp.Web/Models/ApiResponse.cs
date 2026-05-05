namespace HotelApp.Web.Models;

// ApiResponse sınıfının amacı, ApiService sonuçlarını standart hale getirmektir.
// Başarı durumu, veri, mesaj ve HTTP status code bilgisini tek modelde taşır.
// SOLID: SRP kullanır; sadece API istek sonuç verisini temsil eder.

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
