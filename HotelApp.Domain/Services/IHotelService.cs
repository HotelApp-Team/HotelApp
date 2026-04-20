using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelListDto>> GetAllAsync();
        Task<HotelDetailDto?> GetByIdAsync(int id);
        Task<IEnumerable<HotelListDto>> GetByCityAsync(string city);
        Task<IEnumerable<HotelListDto>> GetActiveAsync();
        Task CreateAsync(HotelCreateDto dto);
        Task UpdateAsync(HotelUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
