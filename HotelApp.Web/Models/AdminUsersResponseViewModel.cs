namespace HotelApp.Web.Models;

// AdminUsersResponseViewModel sınıfının amacı, API'den gelen E37 kullanıcı listesini özet sayaçlarla karşılamaktır.
// Liste sayfalı gelir; aktif, pasif ve admin sayıları sayfadan bağımsız toplam değerleri temsil eder.
// SOLID: SRP kullanır; sadece admin kullanıcı liste cevabını web tarafında taşır.
public class AdminUsersResponseViewModel : PagedResponseViewModel<AdminUserViewModel>
{
    public int ActiveCount { get; set; }
    public int PassiveCount { get; set; }
    public int AdminCount { get; set; }
}
