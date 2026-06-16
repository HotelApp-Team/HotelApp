namespace HotelApp.Web.Models
{
    // Admin yorum yonetim ekraninda her bir yorum satirini gostermek icin kullanilir.
    // API'den gelen ReviewDto alanlariyla birebir uyumludur.
    public class AdminReviewViewModel
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
