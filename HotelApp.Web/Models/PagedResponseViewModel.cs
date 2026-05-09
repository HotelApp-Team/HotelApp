namespace HotelApp.Web.Models;

// PagedResponseViewModel sınıfının amacı, backend'den gelen sayfalı liste cevaplarını web tarafında karşılamaktır.
// Farklı liste ekranları T tipini değiştirerek aynı sayfalama modelini tekrar kullanır.
// SOLID: SRP ve OCP kullanır; sayfalama yapısı tek sorumluluktur, yeni liste tipleri generic T ile eklenir.
public class PagedResponseViewModel<T>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Data { get; set; } = new();
}
