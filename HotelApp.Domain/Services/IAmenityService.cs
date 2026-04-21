using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IAmenityService
    {
        Task<List<AmenityDto>> GetAllAsync();
        Task<AmenityDto?> GetByIdAsync(int id);

        Task AddAsync(CreateAmenityDto dto);
        Task UpdateAsync(UpdateAmenityDto dto);
        Task DeleteAsync(int id);

        Task<List<AmenityDto>> GetAmenitiesByHotelIdAsync(int hotelId);

        Task AddAmenityToHotelAsync(int hotelId, int amenityId);
        Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId);
    }
}
