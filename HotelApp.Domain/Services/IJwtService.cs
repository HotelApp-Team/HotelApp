using HotelApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp.Domain.Services
{
    // IJwtService arayüzünün amacı, JWT token üretimi için sözleşme tanımlamaktır.
    // Kullanıcı ve RememberMe bilgisine göre token üretme metodunu belirler.
    // SOLID: ISP kullanır; sadece token üretme ihtiyacını içerir.
    // SOLID: DIP sağlar; AuthService somut JwtService yerine bu arayüze bağlı kalır.

    public interface IJwtService
    {
        //Generate token üretirken aynı zamanda token'ın ne zaman geçersiz olacağını da döndürür
        //Token imzasina RememberMe özelliği
        string GenerateToken(User user, bool rememberMe, out DateTime expiresAt);
    }
}
