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

        Task AddAsync(int hotelId, HotelImageCreateDto dto);

        Task UpdateAsync(int hotelId, HotelImageUpdateDto dto);

        Task DeleteAsync(int hotelId, int imageId);

        Task SetMainImageAsync(int hotelId, int imageId);
    }
}
