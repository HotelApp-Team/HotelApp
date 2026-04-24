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

        Task AddAsync(AmenityCreateDto dto);
        Task UpdateAsync(AmenityUpdateDto dto);
        Task DeleteAsync(int id);

        Task<List<AmenityDto>> GetAmenitiesByHotelIdAsync(int hotelId);

        Task<HotelAmenityAssignResponseDto> AddAmenitiesToHotelAsync(int hotelId, List<int> amenityIds);
        Task RemoveAmenityFromHotelAsync(int hotelId, int amenityId);
    }
}
