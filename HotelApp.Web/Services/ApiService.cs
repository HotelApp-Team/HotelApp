using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HotelApp.Web.Services;

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

    // Her istekte JWT token varsa header'a ekle

    private void SetAuthHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
        if (!string.IsNullOrEmpty(token))

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    //GET istegi atarken T tipinde bir nesne doner
    public async Task<T?> GetTAsync<T>(string endpoint)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode) return default;
        
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    //POST istegi atarken T tipinde bir nesne doner
    public async Task<T?> PostTAsync<T>(string endpoint, object data)
    {
        SetAuthHeader();
        var content = new StringContent(
            JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);
        if(!response.IsSuccessStatusCode) return default;
        
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    //PUT istegi atarken T tipinde bir nesne doner
    public async Task<bool> PutAsync(string endpoint, object data)
    {
        SetAuthHeader();
        var content = new StringContent (
            JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(endpoint, content);
        return response.IsSuccessStatusCode;
    }

    //DELETE istegi atarken T tipinde bir nesne doner
    public async Task<bool> DeleteAsync(string endpoint)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync(endpoint);
        return response.IsSuccessStatusCode;
    }

}

