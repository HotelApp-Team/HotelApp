using HotelApp.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HotelApp.Web.Services;

// ApiService sınıfının amacı, MVC web projesi ile Web API arasında köprü olmaktır.
// GET/POST/PUT/DELETE isteklerini gönderir, JWT token ekler ve API cevaplarını standart ApiResponse modeline çevirir.
// SOLID: SRP kullanır; sadece API iletişimi ve response dönüştürme işlerinden sorumludur.

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    // Session'da token varsa Authorization header'a ekler.
    // Token yoksa eski header temizlenir ki onceki kullanicidan veri kalmasin.
    private void SetAuthHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    // API basarisiz dondugunde once body icindeki standart hata mesajini okur.
    // Body yoksa veya parse edilemezse status code'a gore kullanici dostu Turkce mesaj uretir.
    // Boylece "Durum kodu: 401" gibi teknik metinler kullaniciya gosterilmez.
    private async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(json, _jsonOptions);

                if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                    return errorResponse.Message;
            }
            catch (JsonException)
            {
                // Hata cevabi bizim standart modelimize uymuyorsa asagidaki status mesajina dusulur.
            }
        }

        return GetFriendlyStatusMessage(response.StatusCode);
    }

    // Status code'a karsilik kullaniciya gosterilecek nazik metni doner.
    // 401/403 ozellikle teknik gozukmesin diye Trello S5-02 kapsaminda buraya tasindi.
    private static string GetFriendlyStatusMessage(System.Net.HttpStatusCode statusCode)
    {
        return (int)statusCode switch
        {
            401 => "Bu islem icin once giris yapmaniz gerekiyor.",
            403 => "Bu islemi yapma yetkiniz yok.",
            404 => "Aradiginiz kayit bulunamadi.",
            408 => "Sunucu beklenenden uzun surede cevap verdi. Lutfen tekrar deneyin.",
            429 => "Cok fazla istek gonderdiniz. Lutfen biraz sonra tekrar deneyin.",
            >= 500 => "Sunucuda bir sorun olustu. Lutfen daha sonra tekrar deneyin.",
            _ => "Istek tamamlanamadi. Lutfen tekrar deneyin."
        };
    }

    // Ortak basarili response nesnesi olusturur.
    private static ApiResponse<T> CreateSuccessResponse<T>(T? data, HttpResponseMessage response)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            StatusCode = (int)response.StatusCode
        };
    }

    // Ortak hatali response nesnesi olusturur.
    private static ApiResponse<T> CreateErrorResponse<T>(string message, HttpResponseMessage response)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = (int)response.StatusCode
        };
    }

    // Obje gonderilecek isteklerde JSON content hazirlar.
    private static StringContent CreateJsonContent(object data)
    {
        return new StringContent(
            JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
    }

    // Basarili response body'si varsa T tipine cevirir.
    // Body bos ise Success=true doner, Data default kalir.
    private async Task<ApiResponse<T>> ReadSuccessResponseAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
            return CreateSuccessResponse<T>(default, response);

        var data = JsonSerializer.Deserialize<T>(json, _jsonOptions);
        return CreateSuccessResponse(data, response);
    }

    // Yeni ve temiz GET metodu.
    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        SetAuthHeader();

        var response = await _httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var message = await GetErrorMessageAsync(response);
            return CreateErrorResponse<T>(message, response);
        }

        return await ReadSuccessResponseAsync<T>(response);
    }

    // Yeni ve temiz POST metodu.
    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
    {
        SetAuthHeader();

        var content = CreateJsonContent(data);
        var response = await _httpClient.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await GetErrorMessageAsync(response);
            return CreateErrorResponse<T>(message, response);
        }

        return await ReadSuccessResponseAsync<T>(response);
    }

    // PUT sonunda veri donen endpoint'ler icin generic versiyon.
    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        SetAuthHeader();

        var content = CreateJsonContent(data);
        var response = await _httpClient.PutAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await GetErrorMessageAsync(response);
            return CreateErrorResponse<T>(message, response);
        }

        return await ReadSuccessResponseAsync<T>(response);
    }

    // PUT sonunda sadece basari bilgisi gerekiyorsa bu kisayol kullanilabilir.
    public async Task<ApiResponse<bool>> PutAsync(string endpoint, object data)
    {
        SetAuthHeader();

        var content = CreateJsonContent(data);
        var response = await _httpClient.PutAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await GetErrorMessageAsync(response);
            return CreateErrorResponse<bool>(message, response);
        }

        return CreateSuccessResponse(true, response);
    }

    // DELETE icin sadece islem sonucu yeterlidir.
    public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
    {
        SetAuthHeader();

        var response = await _httpClient.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            var message = await GetErrorMessageAsync(response);
            return CreateErrorResponse<bool>(message, response);
        }

        return CreateSuccessResponse(true, response);
    }

    // Eski kodda GetTAsync/PostTAsync kullaniliyordu (o imza Task<T?> donuyordu).
    // Simdiki imza Task<ApiResponse<T>> oldugu icin tam geriye donuk uyumluluk SAGLAMAZ;
    // sadece eski cagri isimleri bulunabilsin diye alias olarak tutuluyor.
    // Yeni yazilan kod GetAsync/PostAsync kullanmali.
    public Task<ApiResponse<T>> GetTAsync<T>(string endpoint) => GetAsync<T>(endpoint);

    public Task<ApiResponse<T>> PostTAsync<T>(string endpoint, object data) => PostAsync<T>(endpoint, data);
}
