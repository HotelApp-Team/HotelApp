namespace HotelApp.Web.Models;

public class PaginationModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string BaseUrl { get; set; } =null!;
}
