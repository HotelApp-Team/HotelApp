namespace HotelApp.Web.Models
{
    public class RoomImageViewModel
    {
        public int Id { get; set; }
        public int HotelRoomId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }
}
