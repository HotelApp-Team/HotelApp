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
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Tüm olanakları getirir
        public async Task<List<AmenityDto>> GetAllAsync()
        {
            var amenities = await _unitOfWork.AmenityRepository.GetAllAsync();

            return amenities.Select(x => new AmenityDto
            {
                Id = x.Id,
                Name = x.Name,
                Icon = x.Icon
            }).ToList();
        }

        // Id’ye göre olanak getirir
        public async Task<AmenityDto?> GetByIdAsync(int id)
        {
            var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(id);

            if (amenity == null) return null;

            return new AmenityDto
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Icon = amenity.Icon
            };
        }

        // Yeni olanak ekler
        public async Task AddAsync(CreateAmenityDto dto)
        {
            var entity = new Amenity
            {
                Name = dto.Name,
                Icon = dto.Icon
            };

            await _unitOfWork.AmenityRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(); // Değişiklikleri kaydeder
        }

        // Olanak günceller
        public async Task UpdateAsync(UpdateAmenityDto dto)
        {
            var amenity = await _unitOfWork.AmenityRepository.GetByIdAsync(dto.Id);

            if (amenity == null)
                throw new Exception("Olanak bulunamadı");

            amenity.Name = dto.Name;
            amenity.Icon = dto.Icon;

            await _unitOfWork.AmenityRepository.UpdateAsync(amenity);
            await _unitOfWork.SaveChangesAsync();
        }

        // Olanak siler
        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.AmenityRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        //  Otele ait olanakları getirir
        public async Task<List<AmenityDto>> GetAmenitiesByHotelIdAsync(int hotelId)
        {
            var amenities = await _unitOfWork.AmenityRepository.GetAmenitiesByHotelIdAsync(hotelId);

            return amenities.Select(x => new AmenityDto
            {
                Id = x.Id,
                Name = x.Name,
                Icon = x.Icon
            }).ToList();
        }

        // Otele olanak ekler
        public async Task AddAmenityToHotelAsync(int hotelId, int amenityId)
        {
            await _unitOfWork.AmenityRepository.AddAmenityToHotelAsync(hotelId, amenityId);
            await _unitOfWork.SaveChangesAsync();
        }

        // Otelden olanak kaldırır
        public async Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId)
        {
            await _unitOfWork.AmenityRepository.RemoveAmenityFromHotelAsync(hotelId, amenityId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
