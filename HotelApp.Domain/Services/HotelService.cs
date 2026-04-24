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
            return await _unitOfWork.HotelRepository.GetActiveCitiesAsync();
        }
        

        public async Task<HotelSearchResponseDto> SearchAsync(HotelSearchRequestDto dto, int page, int pageSize)
        {
            // TODO (E07 - Availability Search):
            // Sevde'nin sp_SearchAvailableRooms PR'i merge edildikten sonra
            // CheckIn, CheckOut ve GuestCount gerçek müsaitlik kontrolüne bağlanacak.
            // Şu an bu alanlar sadece placeholder olarak duruyor.

            ValidatePaging(page, pageSize);

            var query = _unitOfWork.HotelRepository.Query();

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

        public async Task<object> GetAllFilteredAsync(string? city, byte? star, string? sortBy, string? sortOrder, int page, int pageSize)
        {
            // 1. data çek
            ValidatePaging(page, pageSize);

            var query = _unitOfWork.HotelRepository.Query();

            // 2. FILTER
            if (!string.IsNullOrEmpty(city))
                query = query.Where(x => x.City.ToLower() == city.ToLower());

            if (star.HasValue)
                query = query.Where(x => x.StarRating == star);

            // 3. SORT
            query = (sortBy?.ToLower(), sortOrder?.ToLower()) switch
            {
                ("star", "desc") => query.OrderByDescending(x => x.StarRating),
                ("star", _) => query.OrderBy(x => x.StarRating),

                ("rating", "desc") => query.OrderByDescending(x => x.AvgRating),
                ("rating", _) => query.OrderBy(x => x.AvgRating),

                (_, "desc") => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Name)
            };

            // 4. COUNT
            var totalCount = query.Count();

            // 5. PAGING
            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new HotelListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    City = x.City,
                    Country = x.Country,
                    StarRating = x.StarRating,
                    AvgRating = x.AvgRating,
                    IsActive = x.IsActive,
                    MainImageUrl = x.HotelImages
                    .Where(i => i.IsMain)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()
                })
                .ToList();

            return new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = data
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
