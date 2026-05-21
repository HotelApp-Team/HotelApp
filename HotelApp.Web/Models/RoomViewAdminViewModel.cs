namespace HotelApp.Web.Models
{
    // Admin tarafinda oda formundaki manzara dropdown'i icin kullanilir.
    // API'den /api/roomviews ile cekilir, ApiService deserialize eder.
    public class RoomViewAdminViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
    }
}
