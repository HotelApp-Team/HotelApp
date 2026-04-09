using HotelApp.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(UserRegisterDto registerRequest);
        Task<LoginResponseDto> LoginAsync(UserLoginDto loginRequest);

    }
}
