using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// IJwtService, token uretme davranisinin sozlesmesini tanimlar.
// Amaci, auth akisinin somut JWT implementasyonuna bagimli kalmamasidir.
// SOLID: DIP ve ISP kullanir.
namespace HotelApp.Domain.Services
{
    public interface IJwtService
    {
        //Generate token üretirken aynı zamanda token'ın ne zaman geçersiz olacağını da döndürür
        //Token imzasina RememberMe özelliği
        string GenerateToken(User user, bool rememberMe, out DateTime expiresAt);
    }
}
