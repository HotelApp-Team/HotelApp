namespace HotelApp.Domain.DTOs
{
    // Admin dropdown'i ve oda kartlarinda manzara gostermek icin kullanilan DTO.
    // RoomView lookup tablosundan dondurulur; UI tarafinda secim listesi olusturur.
    public class RoomViewDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Icon { get; set; }

        public int DisplayOrder { get; set; }
    }
}
