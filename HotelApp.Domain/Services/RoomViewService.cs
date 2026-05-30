using HotelApp.Domain.DTOs;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    // Oda manzara listesini DB'den okuyup DTO'ya cevirir.
    // CRUD yok cunku manzara turleri seed ile sabit (Deniz, Sehir, Dag vb.).
    public class RoomViewService : IRoomViewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomViewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoomViewDto>> GetAllAsync()
        {
            // Tum manzaralari cek, sadece aktif olanlari al, DisplayOrder ile sirala.
            var views = await _unitOfWork.RoomViewRepository.GetAllAsync();

            return views
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new RoomViewDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Icon = x.Icon,
                    DisplayOrder = x.DisplayOrder
                });
        }
    }
}
