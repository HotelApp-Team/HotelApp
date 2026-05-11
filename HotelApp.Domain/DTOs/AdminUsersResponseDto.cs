namespace HotelApp.Domain.DTOs;

// AdminUsersResponseDto sınıfının amacı, E37 kullanıcı listesini özet sayaçlarla birlikte döndürmektir.
// Liste sayfalı gelir; aktif, pasif ve admin sayıları ise sayfadan bağımsız toplam değerleri taşır.
// SOLID: SRP kullanır; sadece admin kullanıcı liste cevabının veri taşıma sorumluluğunu üstlenir.
public class AdminUsersResponseDto : PagedResponseDto<AdminUserDto>
{
    public int ActiveCount { get; set; }
    public int PassiveCount { get; set; }
    public int AdminCount { get; set; }
}
