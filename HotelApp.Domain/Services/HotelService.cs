using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HotelService(IHotelRepository hotelRepository, IUnitOfWork unitOfWork)
        {
            _hotelRepository = hotelRepository;
            _unitOfWork = unitOfWork;
        }

        // Tüm oteller (Liste DTO)
        public async Task<IEnumerable<HotelListDto>> GetAllAsync()
        {
            var hotels = await _hotelRepository.GetAllAsync();

            return hotels.Select(x => new HotelListDto
            {
                Id = x.Id,
                Name = x.Name,
                City = x.City,
                Country = x.Country,
                StarRating = x.StarRating,
                AvgRating = x.AvgRating,
                IsActive = x.IsActive
            });
        }

        // Detay
        public async Task<HotelDetailDto?> GetByIdAsync(int id)
        {
            var hotel = await _hotelRepository.GetWithDetailsAsync(id);

            if (hotel == null)
                return null;

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

                ImageUrls = hotel.HotelImages.Select(i => i.ImageUrl).ToList(),
                Amenities = hotel.Amenities.Select(a => a.Name).ToList()
            };
        }

        // Şehre göre filtre
        public async Task<IEnumerable<HotelListDto>> GetByCityAsync(string city)
        {
            var hotels = await _hotelRepository.GetByCityAsync(city);

            return hotels.Select(x => new HotelListDto
            {
                Id = x.Id,
                Name = x.Name,
                City = x.City,
                Country = x.Country,
                StarRating = x.StarRating,
                AvgRating = x.AvgRating,
                IsActive = x.IsActive
            });
        }

        // Aktif oteller
        public async Task<IEnumerable<HotelListDto>> GetActiveAsync()
        {
            var hotels = await _hotelRepository.GetActiveAsync();

            return hotels.Select(x => new HotelListDto
            {
                Id = x.Id,
                Name = x.Name,
                City = x.City,
                Country = x.Country,
                StarRating = x.StarRating,
                AvgRating = x.AvgRating,
                IsActive = x.IsActive
            });
        }

        // Create
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

            await _hotelRepository.AddAsync(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        // Update
        public async Task UpdateAsync(HotelUpdateDto dto)
        {
            var hotel = await _hotelRepository.GetByIdAsync(dto.Id);

            if (hotel == null)
                throw new Exception("Otel bulunamadı");

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

            _hotelRepository.Update(hotel);
            await _unitOfWork.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteAsync(int id)
        {
            var hotel = await _hotelRepository.GetByIdAsync(id);

            if (hotel == null)
                throw new Exception("Otel bulunamadı");

            hotel.IsActive = false;
            hotel.UpdatedAt = DateTime.UtcNow;

            _hotelRepository.Update(hotel);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
