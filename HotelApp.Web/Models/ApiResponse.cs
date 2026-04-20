namespace HotelApp.Web.Models;

// ApiResponse, ApiService'in her istekte standart sonuc dondurmesi icin kullanilir.
// Basarili durumda Data dolar, hatada Message ve StatusCode bilgisi tasinir.
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
