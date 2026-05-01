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

            if (hotel == null || !hotel.IsActive)
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
            return await _unitOfWork.HotelRepository.GetActiveCitiesAsync();
        }


        public async Task<HotelSearchResponseDto> SearchAsync(HotelSearchRequestDto dto, int page, int pageSize)
        {
            ValidatePaging(page, pageSize);

            var hasAvailabilityFilter =
                dto.CheckIn.HasValue ||
                dto.CheckOut.HasValue ||
                dto.GuestCount.HasValue;

            if (!hasAvailabilityFilter)
            {
                var (data, totalCount) =
                    await _unitOfWork.HotelRepository.SearchPagedAsync(
                        dto.City,
                        dto.StarRating,
                        dto.MinRating,
                        page,
                        pageSize);

                return new HotelSearchResponseDto
                {
                    TotalCount = totalCount,
                    Hotels = data.Select(MapToListDto).ToList()
                };
            }

            if (!dto.CheckIn.HasValue || !dto.CheckOut.HasValue || !dto.GuestCount.HasValue)
                throw new ArgumentException("Müsaitlik araması için CheckIn, CheckOut ve GuestCount zorunludur.");

            if (dto.CheckIn.Value >= dto.CheckOut.Value)
                throw new ArgumentException("CheckIn tarihi CheckOut tarihinden küçük olmalıdır.");

            if (dto.GuestCount.Value < 1)
                throw new ArgumentException("GuestCount en az 1 olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.City))
                throw new ArgumentException("Müsaitlik araması için şehir bilgisi zorunludur.");

            var candidateHotels = _unitOfWork.HotelRepository.Query()
                .Where(h => h.IsActive)
                .Where(h => h.City == dto.City)
                .Where(h => !dto.StarRating.HasValue || h.StarRating == dto.StarRating.Value)
                .Where(h => !dto.MinRating.HasValue || (h.AvgRating.HasValue && h.AvgRating.Value >= dto.MinRating.Value))
                .Select(h => new HotelListDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    City = h.City,
                    Country = h.Country,
                    StarRating = h.StarRating,
                    AvgRating = h.AvgRating,
                    IsActive = h.IsActive,
                    MainImageUrl = h.HotelImages
                        .Where(i => i.IsMain)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .ToList();

            var availableHotels = new List<HotelListDto>();

            foreach (var hotel in candidateHotels)
            {
                var availableRooms = await _unitOfWork.HotelRoomRepository.GetAvailableRoomsAsync(
                    hotel.Id,
                    dto.City,
                    dto.CheckIn.Value,
                    dto.CheckOut.Value,
                    dto.GuestCount.Value);

                if (availableRooms.Any())
                {
                    availableHotels.Add(hotel);
                }
            }

            var availableTotalCount = availableHotels.Count;

            var pagedHotels = availableHotels
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new HotelSearchResponseDto
            {
                TotalCount = availableTotalCount,
                Hotels = pagedHotels
            };
        }

        public async Task<PagedResponseDto<HotelListDto>> GetAllFilteredAsync(string? city, byte? star, string? sortBy, string? sortOrder, int page, int pageSize)
        {
            var (data, totalCount) =
                await _unitOfWork.HotelRepository.GetPagedFilteredAsync(
                    city, star, sortBy, sortOrder, page, pageSize);

            var mapped = data.Select(MapToListDto).ToList();

            return new PagedResponseDto<HotelListDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = mapped
            };
        }
        private void ValidatePaging(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentException("Page 1 veya daha büyük olmalıdır.");

            if (pageSize < 1)
                throw new ArgumentException("PageSize 1 veya daha büyük olmalıdır.");

            if (pageSize > 100)
                throw new ArgumentException("PageSize en fazla 100 olabilir.");
        }
    }
}
