using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.Web.Helpers;

// SessionAuthExtensions sınıfının amacı, session içindeki JWT tokenın geçerli olup olmadığını kontrol etmektir.
// Token yoksa, bozuksa veya süresi dolmuşsa kullanıcıyı oturum açmamış kabul eder ve session bilgisini temizler.
// SOLID: SRP kullanır; sadece session authentication yardımcı işleminden sorumludur.

public static class SessionAuthExtensions
{
    public const string JwtSessionKey = "JWToken";
    public const string UserEmailSessionKey = "UserEmail";
    public const string UserFullNameSessionKey = "UserFullName";
    public const string RememberMeCookieName = "HotelApp.RememberMeToken";

    public static bool IsAuthenticated(this ISession session)
    {
        var token = session.GetString(JwtSessionKey);
        // Token hiç yok: kullanıcı giriş yapmamış.
        if(string.IsNullOrEmpty(token))
            return false;

        var handler = new JwtSecurityTokenHandler();

        // Token formatı JWT'ye uymuyor. Session'ı temizle.
        if(!handler.CanReadToken(token))
        {
            session.ClearAuthSession();
            return false;
        }

        // Token okunabiliyorsa süresini kontrol et.
        var jwt = handler.ReadJwtToken(token);
        if(jwt.ValidTo < DateTime.UtcNow)
        {
            // Token süresi dolmuş. Auth session bilgilerini temizle.
            session.ClearAuthSession();
            return false;
        }
        return true;
    }

    // Login veya email doğrulama başarılı olduğunda JWT ve kullanıcı özet bilgisini session'a yazar.
    public static void SetAuthSession(this ISession session, string token, string email, string fullName)
    {
        session.SetString(JwtSessionKey, token);
        session.SetString(UserEmailSessionKey, email);
        session.SetString(UserFullNameSessionKey, fullName);
    }

    // Logout veya geçersiz token durumunda sadece auth ile ilgili session alanlarını temizler.
    public static void ClearAuthSession(this ISession session)
    {
        session.Remove(JwtSessionKey);
        session.Remove(UserEmailSessionKey);
        session.Remove(UserFullNameSessionKey);
    }

    // Beni Hatırla cookie'si varsa geçerli tokenı session'a geri yükler.
    // Böylece tarayıcı kapansa bile geçerli JWT süresi boyunca kullanıcı tekrar login olmadan devam eder.
    public static void RestoreAuthSessionFromRememberMeCookie(this HttpContext context)
    {
        // Session zaten geçerliyse cookie'den tekrar yükleme yapmaya gerek yok.
        if (context.Session.IsAuthenticated())
            return;

        // Cookie yoksa veya boşsa kullanıcı anonim kabul edilir.
        if (!context.Request.Cookies.TryGetValue(RememberMeCookieName, out var token) || string.IsNullOrWhiteSpace(token))
            return;

        var handler = new JwtSecurityTokenHandler();

        // Cookie'deki değer JWT formatında değilse güvenli tarafta kalıp cookie'yi siler.
        if (!handler.CanReadToken(token))
        {
            context.Response.Cookies.Delete(RememberMeCookieName);
            return;
        }

        var jwt = handler.ReadJwtToken(token);

        // Token süresi dolduysa eski cookie silinir ve session yüklenmez.
        if (jwt.ValidTo < DateTime.UtcNow)
        {
            context.Response.Cookies.Delete(RememberMeCookieName);
            return;
        }

        // Session'a yazılacak kullanıcı özet bilgileri JWT claim'lerinden okunur.
        var email = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email || c.Type == ClaimTypes.Email)?.Value;
        var fullName = jwt.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? email;

        // Email veya ad bilgisi yoksa navbar/session eksik çalışmasın diye cookie temizlenir.
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fullName))
        {
            context.Response.Cookies.Delete(RememberMeCookieName);
            return;
        }

        context.Session.SetAuthSession(token, email, fullName);
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

        var token = session.GetString(JwtSessionKey)!;
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
