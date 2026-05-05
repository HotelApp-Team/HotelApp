using System.IdentityModel.Tokens.Jwt;

namespace HotelApp.Web.Helpers;

// SessionAuthExtensions sınıfının amacı, session içindeki JWT tokenın geçerli olup olmadığını kontrol etmektir.
// Token yoksa, bozuksa veya süresi dolmuşsa kullanıcıyı oturum açmamış kabul eder ve session bilgisini temizler.
// SOLID: SRP kullanır; sadece session authentication yardımcı işleminden sorumludur.

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
