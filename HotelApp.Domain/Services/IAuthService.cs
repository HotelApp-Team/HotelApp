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
        // Yeni kullanici kaydi olusturur ve sonucunda login response dondurur
        Task<LoginResponseDto> RegisterAsync(UserRegisterDto registerRequest);

        // Kullanici girisi yapar, bilgiler dogruysa JWT token iceren response dondurur
        Task<LoginResponseDto> LoginAsync(UserLoginDto loginRequest);

    }
}
