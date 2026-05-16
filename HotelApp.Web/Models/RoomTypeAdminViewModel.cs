namespace HotelApp.Web.Models
{
    // Admin panelinde oda tipi listeleme, ekleme ve güncelleme işlemleri için kullanılır.

    public class RoomTypeAdminViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxGuests { get; set; }
    }
}
