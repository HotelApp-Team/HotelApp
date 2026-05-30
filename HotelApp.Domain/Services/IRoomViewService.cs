using HotelApp.Domain.DTOs;

namespace HotelApp.Domain.Services
{
    // Oda manzara listesini sunan service sozlesmesi.
    // Seed-only liste oldugu icin CRUD yok; sadece okuma yapilir.
    public interface IRoomViewService
    {
        // Admin dropdown'i icin tum aktif manzara turlerini getirir.
        // Donen liste DisplayOrder ASC sirali olur.
        Task<IEnumerable<RoomViewDto>> GetAllAsync();
    }
}
