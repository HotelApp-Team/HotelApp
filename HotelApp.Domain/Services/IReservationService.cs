using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId);
        Task<IEnumerable<ReservationDetailDto>> GetUserReservationDetailsAsync(int userId);
        Task<ReservationDto?> GetReservationByIdAsync(int id);
        Task<ReservationDto> CreateReservationAsync(int userId, CreateReservationDto dto);
        Task CancelReservationAsync(int id, int userId, bool isAdmin);
    }
}
