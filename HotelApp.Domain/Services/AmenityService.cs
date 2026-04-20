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
            var exists = await _unitOfWork.AmenityRepository
         .ExistsAsync(x => x.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new InvalidOperationException("Bu olanak zaten mevcut");

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

            var exists = await _unitOfWork.AmenityRepository
                .ExistsAsync(x => x.Name.ToLower() == dto.Name.ToLower()
                               && x.Id != dto.Id);

            if (exists)
                throw new InvalidOperationException("Bu isimde bir olanak zaten var");


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

            var isUsed = await _unitOfWork.HotelRepository
                .ExistsAsync(h => h.Amenities.Any(a => a.Id == id));


            if (isUsed)
                throw new InvalidOperationException("Bu olanak oteller tarafından kullanılıyor, silinemez");

            _unitOfWork.AmenityRepository.Delete(amenity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<AmenityDto>> GetAmenitiesByHotelIdAsync(int hotelId)
        {
            var amenities = await _unitOfWork.AmenityRepository.GetAmenitiesByHotelIdAsync(hotelId);
            return amenities.Select(MapToDto).ToList();
        }

        public async Task AddAmenitiesToHotelAsync(int hotelId, List<int> amenityIds)
        {
            var hotel = await _unitOfWork.HotelRepository
                .GetWithDetailsAsync(hotelId);

            if (hotel == null)
                throw new KeyNotFoundException("Otel bulunamadı");

            foreach (var amenityId in amenityIds)
            {
                var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(amenityId);

                if (amenity == null)
                    continue;

                // duplicate prevent
                if (!hotel.Amenities.Any(a => a.Id == amenityId))
                {
                    hotel.Amenities.Add(amenity);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId)
        {
            var hotel = await _unitOfWork.HotelRepository
                .GetWithDetailsAsync(hotelId);

            if (hotel == null)
                throw new KeyNotFoundException("Otel bulunamadı");

            var amenity = hotel.Amenities.FirstOrDefault(a => a.Id == amenityId);

            if (amenity == null)
                throw new KeyNotFoundException("Olanak bu otele ait değil");

            hotel.Amenities.Remove(amenity);

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
