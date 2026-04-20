namespace HotelApp.Web.Models;

// ApiErrorResponse, API tarafindan donen standart hata cevabini okumak icin kullanilir.
public class ApiErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
