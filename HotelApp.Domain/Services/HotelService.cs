using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;

namespace HotelApp.Domain.Services
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HotelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<HotelListDto>> GetAllAsync()
        {
            var hotels = await _unitOfWork.HotelRepository.GetAllAsync();
            return hotels.Select(MapToListDto);
        }

        public async Task<HotelDetailDto?> GetByIdAsync(int id)
        {
            var hotel = await _unitOfWork.HotelRepository.GetWithDetailsAsync(id);

            if (hotel == null)
            {
                return null;
            }

            return new HotelDetailDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                Phone = hotel.Phone,
                Email = hotel.Email,
                Latitude = hotel.Latitude,
                Longitude = hotel.Longitude,
                StarRating = hotel.StarRating,
                AvgRating = hotel.AvgRating,
                IsActive = hotel.IsActive,
                Amenities = hotel.Amenities.Select(a => new AmenityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Icon = a.Icon
                }).ToList()
            };
        }

        public async Task<IEnumerable<HotelListDto>> GetByCityAsync(string city)
        {
            var hotels = await _unitOfWork.HotelRepository.GetByCityAsync(city);
            return hotels.Select(MapToListDto);
        }

        public async Task<IEnumerable<HotelListDto>> GetActiveAsync()
        {
            var hotels = await _unitOfWork.HotelRepository.GetActiveAsync();
            return hotels.Select(MapToListDto);
        }

        public async Task CreateAsync(HotelCreateDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                Phone = dto.Phone,
                Email = dto.Email,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                StarRating = dto.StarRating,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.HotelRepository.AddAsync(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(HotelUpdateDto dto)
        {
            var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(dto.Id);

            if (hotel == null)
            {
                throw new KeyNotFoundException("Otel bulunamadı");
            }

            hotel.Name = dto.Name;
            hotel.Description = dto.Description;
            hotel.Address = dto.Address;
            hotel.City = dto.City;
            hotel.Country = dto.Country;
            hotel.Phone = dto.Phone;
            hotel.Email = dto.Email;
            hotel.Latitude = dto.Latitude;
            hotel.Longitude = dto.Longitude;
            hotel.StarRating = dto.StarRating;
            hotel.IsActive = dto.IsActive;
            hotel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.HotelRepository.Update(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(id);

            if (hotel == null)
                throw new KeyNotFoundException("Otel bulunamadı");

            if (!hotel.IsActive)
                throw new InvalidOperationException("Otel zaten pasif");

            hotel.IsActive = false;
            hotel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.HotelRepository.Update(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        private static HotelListDto MapToListDto(Hotel hotel)
        {
            return new HotelListDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                City = hotel.City,
                Country = hotel.Country,
                StarRating = hotel.StarRating,
                AvgRating = hotel.AvgRating,
                IsActive = hotel.IsActive,
                MainImageUrl = hotel.HotelImages
                 .FirstOrDefault(x => x.IsMain)?.ImageUrl
            };
        }

        public async Task<List<string>> GetCitiesAsync()
        {
            var hotels = await _unitOfWork.HotelRepository.GetActiveAsync();

            return hotels
                .Select(x => x.City)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public async Task<HotelSearchResponseDto> SearchAsync(HotelSearchRequestDto dto, int page, int pageSize)
        {
            var query = await _unitOfWork.HotelRepository.GetAllAsync();

            // filtering
            if (!string.IsNullOrEmpty(dto.City))
                query = query.Where(x => x.City.ToLower() == dto.City.ToLower());

            if (dto.StarRating.HasValue)
                query = query.Where(x => x.StarRating == dto.StarRating);

            if (dto.MinRating.HasValue)
                query = query.Where(x => x.AvgRating >= dto.MinRating);

            var totalCount = query.Count();

            var paged = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToListDto)
                .ToList();

            return new HotelSearchResponseDto
            {
                TotalCount = totalCount,
                Hotels = paged
            };
        }
    }
}
