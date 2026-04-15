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
        //Generate token üretirken aynı zamanda token'ın ne zaman geçersiz olacağını da döndürür
        //Token imzasina RememberMe özelliği
        string GenerateToken(User user, bool rememberMe, out DateTime expiresAt);
    }
}
