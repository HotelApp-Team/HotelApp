using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IHotelImageService
    {
        Task<List<HotelImageDto>> GetByHotelIdAsync(int hotelId);

        Task<HotelImageDto?> GetByIdAsync(int id);

        Task<HotelImageDto?> GetMainImageAsync(int hotelId);

        Task AddAsync(HotelImageCreateDto dto);

        Task UpdateAsync(HotelImageUpdateDto dto);

        Task DeleteAsync(int id);

        Task SetMainImageAsync(int hotelId, int imageId);
    }
}
