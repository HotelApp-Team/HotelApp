using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AmenityDto>> GetAllAsync()
        {
            var amenities = await _unitOfWork.AmenityRepository.GetAllAsync();
            return amenities.Select(MapToDto).ToList();
        }

        public async Task<AmenityDto?> GetByIdAsync(int id)
        {
            var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(id);

            if (amenity == null)
            {
                return null;
            }

            return MapToDto(amenity);
        }

        public async Task AddAsync(CreateAmenityDto dto)
        {
            var entity = new Amenity
            {
                Name = dto.Name,
                Icon = dto.Icon
            };

            await _unitOfWork.AmenityRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateAmenityDto dto)
        {
            var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(dto.Id);

            if (amenity == null)
            {
                throw new Exception("Olanak bulunamadı");
            }

            amenity.Name = dto.Name;
            amenity.Icon = dto.Icon;

            _unitOfWork.AmenityRepository.Update(amenity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(id);

            if (amenity == null)
            {
                throw new Exception("Olanak bulunamadı");
            }

            _unitOfWork.AmenityRepository.Delete(amenity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<AmenityDto>> GetAmenitiesByHotelIdAsync(int hotelId)
        {
            var amenities = await _unitOfWork.AmenityRepository.GetAmenitiesByHotelIdAsync(hotelId);
            return amenities.Select(MapToDto).ToList();
        }

        public async Task AddAmenityToHotelAsync(int hotelId, int amenityId)
        {
            await _unitOfWork.AmenityRepository.AddAmenityToHotelAsync(hotelId, amenityId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId)
        {
            await _unitOfWork.AmenityRepository.RemoveAmenityFromHotelAsync(hotelId, amenityId);
            await _unitOfWork.SaveChangesAsync();
        }

        private static AmenityDto MapToDto(Amenity amenity)
        {
            return new AmenityDto
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Icon = amenity.Icon
            };
        }
    }
}
