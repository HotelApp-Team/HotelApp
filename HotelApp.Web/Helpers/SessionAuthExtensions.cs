using System.IdentityModel.Tokens.Jwt;

namespace HotelApp.Web.Helpers;

// Session'daki JWT token'in gercekten gecerli olup olmadigini kontrol eden yardimci.
// AuthFilter, AdminFilter ve _LoginPartial bu metodu cagirir.
public static class SessionAuthExtensions
{
    public static bool IsAuthenticated(this ISession session)
    {
        var token = session.GetString("JWToken");
        // Token hiç yok: kullanıcı giriş yapmamış.
        if(string.IsNullOrEmpty(token))
            return false;

        var handler = new JwtSecurityTokenHandler();

        // Token formatı JWT'ye uymuyor. Session'ı temizle.
        if(!handler.CanReadToken(token))
        {
            session.Remove("JWToken");
            session.Remove("UserEmail");
            session.Remove("UserFullName");
            return false;
        }

        // Token okunabiliyorsa süresini kontrol et.
        var jwt = handler.ReadJwtToken(token);
        if(jwt.ValidTo < DateTime.UtcNow)
        {
            // Token süresi dolmuş. Auth session bilgilerini temizle.
            session.Remove("JWToken");
            session.Remove("UserEmail");
            session.Remove("UserFullName");
            return false;
        }
        return true;
    }
}
