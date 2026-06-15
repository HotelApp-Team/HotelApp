using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]

// Otel işlemleri için dışarıya açılan API endpointlerini yönetir.
// Kullanıcı tarafı otel listeleme, detay görüntüleme ve arama işlemlerini buradan yapar.
// Admin işlemleri için oluşturma, güncelleme ve silme endpointleri rol kontrolü ile korunur.
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ILoggerService _logger;

    // HotelService ve LoggerService bağımlılıkları constructor ile alınır.
    public HotelsController(IHotelService hotelService, ILoggerService logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    // E04 - GET /api/hotels (Pagination)
    // Otel listesini filtreleme, sıralama ve sayfalama desteğiyle getirir.
    // Şehir, yıldız ve aktiflik filtreleri opsiyoneldir.
    // Page ve pageSize değerleri geçersizse BadRequest döndürür.
    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] string? city,
    [FromQuery] byte? star,
    [FromQuery] bool? isActive,
    [FromQuery] int? hotelId,
    [FromQuery] string? sortBy = "name",
    [FromQuery] string? sortOrder = "asc",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        if (page < 1)
            return BadRequest(new { Message = "Page 1 veya daha büyük olmalıdır" });

        if (pageSize < 1 || pageSize > 100)
            return BadRequest(new { Message = "PageSize 1 ile 100 arasında olmalıdır" });

        var result = await _hotelService.GetAllFilteredAsync(
            city, star, isActive, hotelId, sortBy, sortOrder, page, pageSize);

        return Ok(result);
    }

    // GET /api/hotels/filter-options
    // Admin otel listesi filtrelerinde kullanilacak sehir ve otel seceneklerini getirir.
    // Aktif + pasif tum oteller dahildir; sehir ve otel dropdownlarini tek istekte besler.
    [HttpGet("filter-options")]
    public async Task<IActionResult> GetFilterOptions()
    {
        var options = await _hotelService.GetFilterOptionsAsync();
        return Ok(options);
    }

    // E05 - GET /api/hotels/{id}
    // Id değerine göre tek bir otelin detay bilgisini getirir.
    // Servis null döndürürse otel bulunamadığı için 404 NotFound cevabı verilir.
    // Otel bulunduğunda detay DTO'su 200 OK ile döndürülür.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);

        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    // GET /api/hotels/catalog-search
    // Otel adı veya şehir bilgisine göre aktif otelleri sayfalı olarak arar.
    [HttpGet("catalog-search")]
    public async Task<IActionResult> CatalogSearch([FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 9)
    {
        // Arama ifadesinin boş gönderilmesini engeller.
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest(new
            {
                Message = "Otel veya şehir bilgisi zorunludur."
            });
        }

        // Sayfa numarasının geçerli olmasını kontrol eder.
        if (page < 1)
        {
            return BadRequest(new
            {
                Message = "Sayfa numarası 1 veya daha büyük olmalıdır."
            });
        }

        // Sayfa boyutunun izin verilen aralıkta olmasını kontrol eder.
        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                Message = "Sayfa boyutu 1 ile 100 arasında olmalıdır."
            });
        }
        // Aramayı service katmanına gönderir.
        var result = await _hotelService.SearchCatalogAsync(
            searchTerm.Trim(), page, pageSize);

        // Sayfalı arama sonucunu kullanıcıya döndürür.
        return Ok(result);
    }

    // E06 - GET /api/hotels/cities
    // Sistemde aktif oteli bulunan şehirleri getirir.
    // Arama formunda şehir seçeneği göstermek için kullanılabilir.
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _hotelService.GetCitiesAsync();
        return Ok(cities);
    }

    // GET /api/hotels/hero-stats : Anasayfa hero alaninda gosterilecek otel ve sehir sayilarini getirir.
    // Ana sayfada gösterilecek toplam otel ve şehir istatistiklerini döndürür.
    [HttpGet("hero-stats")]
    public async Task<IActionResult> GetHeroStats()
    {
        var stats = await _hotelService.GetHeroStatsAsync();
        return Ok(stats);
    }

    // E52 - GET /api/hotels/featured-cities : Anasayfada gosterilecek one cikan sehir kartlarini getirir.
    // Count parametresine göre öne çıkan şehir kartlarını getirir.
    [HttpGet("featured-cities")]
    public async Task<IActionResult> GetFeaturedCities([FromQuery] int count = 5)
    {
        var cities = await _hotelService.GetFeaturedCitiesAsync(count);
        return Ok(cities);
    }

    // E07 - GET /api/hotels/search
    // Kullanıcının şehir, giriş-çıkış tarihi ve misafir sayısına göre otel araması yapar.
    // Servis katmanı tarih, şehir ve sayfalama kontrollerini yapar.
    // Geçersiz arama kriteri varsa BadRequest döndürülür.
    [HttpGet("search")]
    public async Task<IActionResult> Search(
    [FromQuery] HotelSearchRequestDto dto,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _hotelService.SearchAsync(dto, page, pageSize);

            await LogInfoAsync(
                $"Otel arama yapıldı.");

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                Message = ex.Message
            });
        }
    }

    // E08 - POST /api/hotels
    // Yeni otel oluşturmak için kullanılır.
    // Sadece Admin rolündeki kullanıcılar erişebilir.
    // Oluşturma başarılı olursa yeni otelin Id bilgisiyle CreatedAtAction cevabı döner.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] HotelCreateDto dto)
    {
        var hotelId = await _hotelService.CreateAsync(dto);

        await LogInfoAsync(
            $"Yeni otel oluşturuldu: Hotel Adı:{dto.Name}, Şehir:{dto.City}");

        return CreatedAtAction(nameof(GetById), new { id = hotelId }, new { id = hotelId });
    }

    // E09 - PUT /api/hotels
    // Mevcut otel bilgilerini güncellemek için kullanılır.
    // Route içindeki id ile body içindeki dto.Id eşleşmezse BadRequest döndürür.
    // Sadece Admin rolündeki kullanıcılar erişebilir.
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] HotelUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id uyumsuz");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _hotelService.UpdateAsync(dto);

        await LogInfoAsync(
            $"Otel güncellendi: HotelId:{dto.Id}");

        return NoContent();
    }

    // E10 - DELETE /api/hotels/{id}
    // Oteli fiziksel olarak silmek yerine servis katmanında pasif duruma alır.
    // İşlem başarılı olursa 204 NoContent döndürülür.
    // Sadece Admin rolündeki kullanıcılar erişebilir.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _hotelService.DeleteAsync(id);

        await LogInfoAsync(
            $"Otel silindi: HotelId:{id}");

        return NoContent();
    }

    // Ortak bilgi logu atmak icin kullanilir.
    // Mesaj, kullanici bilgisi, controller/action ve IP bilgisini tek yerden logger servisine yollar.
    private async Task LogInfoAsync(string message, int? userId = null, string? userEmail = null)
    {
        await _logger.LogInfoAsync(
            message: message,
            userId: userId,
            userEmail: userEmail,
            controller: nameof(HotelsController),
            action: $"{Request.Method} {Request.Path}",
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString());
    }
}
