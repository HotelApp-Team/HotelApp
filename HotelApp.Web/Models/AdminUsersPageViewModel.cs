namespace HotelApp.Web.Models;

// AdminUsersPageViewModel sınıfının amacı, admin kullanıcı listeleme sayfasının filtre ve sonuç verisini taşımaktır.
// E37 arama, aktiflik ve sayfalama bilgileriyle kullanıcı listesini tek modelde toplar.
// SOLID: SRP kullanır; sadece kullanıcı yönetimi liste ekranının durumunu temsil eder.
public class AdminUsersPageViewModel
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public List<AdminUserViewModel> Users { get; set; } = new();
}
