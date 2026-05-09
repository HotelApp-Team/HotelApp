using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    // Session'daki JWT token'in icindeki rolleri okuyup verilen rol bilgisini iceriyor mu kontrol eder.
    // Token yoksa, bozuksa veya rol yoksa false doner; navbar ve view kararlarinda kullanilir.
    // AdminFilter token decode mantigiyla ayni pattern; tek noktadan rol kontrolu icin merkezilesmistir.
    public static bool IsInRole(this ISession session, string roleName)
    {
        // Once token gecerliligini IsAuthenticated uzerinden dogrula.
        // Bu cagri ayni zamanda suresi dolmus veya bozuk token icin session'i temizliyor;
        // boylece IsInRole sade kalir ve expired token'da yanlislikla "rol var" demez.
        if (!session.IsAuthenticated())
            return false;

        var token = session.GetString("JWToken")!;
        var handler = new JwtSecurityTokenHandler();

        // IsAuthenticated CanReadToken kontrolunu zaten yapti; yine de defensive guard.
        try
        {
            var jwt = handler.ReadJwtToken(token);

            // JWT icindeki rol claim'leri ClaimTypes.Role veya kisa "role" anahtariyla gelebilir.
            // Karsilastirma case-insensitive yapilir ki "admin" / "Admin" farki sorun olmasin.
            return jwt.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Any(c => string.Equals(c.Value, roleName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            // ReadJwtToken beklenmedik exception atarsa view render bozulmasin; sessizce false don.
            return false;
        }
    }
}
