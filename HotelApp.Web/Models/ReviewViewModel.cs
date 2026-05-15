namespace HotelApp.Web.Models
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
