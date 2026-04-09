using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user, out DateTime expiresAt); //Generate token üretirken aynı zamanda token'ın ne zaman geçersiz olacağını da döndürür
    }
}
