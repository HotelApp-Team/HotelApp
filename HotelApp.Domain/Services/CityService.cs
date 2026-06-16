using HotelApp.Domain.DTOs;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    // Sehir listesini DB'den okuyup DTO'ya cevirir.
    // CRUD yok cunku iller seed ile sabittir.
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CityDto>> GetAllAsync()
        {
            var cities = await _unitOfWork.CityRepository.GetAllAsync();

            return cities
                .OrderBy(x => x.Name)
                .Select(x => new CityDto
                {
                    Id = x.Id,
                    Name = x.Name
                });
        }
    }
}
