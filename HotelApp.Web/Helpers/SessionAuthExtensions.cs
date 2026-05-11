using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

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

    // Token gecerliligini imza + issuer + audience + sure dahil tam dogrular.
    // IConfiguration parametresi appsettings'teki Jwt:SecretKey/Issuer/Audience'a erismek icin gerekli.
    // Token yok, bozuk, imza tutmuyor veya suresi dolmussa session temizlenir ve false doner.
    public static bool IsAuthenticated(this ISession session, IConfiguration configuration)
    {
        var token = session.GetString(JwtSessionKey);
        // Token hiç yok: kullanıcı giriş yapmamış.
        if(string.IsNullOrEmpty(token))
            return false;

        // Imza, issuer, audience ve lifetime kontrollerinin hepsi burada yapilir.
        // Sadece ValidTo kontrolu yetmez cunku payload manipule edilmis token da geçebilirdi.
        if (!TryValidateJwtToken(token, configuration, out _))
        {
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
    public static void RestoreAuthSessionFromRememberMeCookie(this HttpContext context, IConfiguration configuration)
    {
        // Session'da token varsa önce imza/issuer/audience kontrolüyle gerçekten geçerli mi bakılır.
        var sessionToken = context.Session.GetString(JwtSessionKey);
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            if (TryValidateJwtToken(sessionToken, configuration, out _))
                return;

            // Session tokenı bozuksa kullanıcı girişli görünmesin diye auth bilgileri temizlenir.
            // Cookie ayrıca aşağıda doğrulanır; geçerliyse session yeniden kurulabilir.
            context.Session.ClearAuthSession();
        }

        // Cookie yoksa veya boşsa kullanıcı anonim kabul edilir.
        if (!context.Request.Cookies.TryGetValue(RememberMeCookieName, out var token) || string.IsNullOrWhiteSpace(token))
            return;

        // Cookie'deki token imza, issuer, audience veya süre kontrolünden geçmezse temizlenir.
        if (!TryValidateJwtToken(token, configuration, out var jwt) || jwt is null)
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

    // JWT token'ın sadece okunabilir olmasını değil, gerçekten bizim ayarlarla imzalanmış olmasını doğrular.
    private static bool TryValidateJwtToken(string token, IConfiguration configuration, out JwtSecurityToken? jwt)
    {
        jwt = null;
        var secretKey = configuration["Jwt:SecretKey"];

        // Secret key yoksa güvenli tarafta kalıp tokenı geçersiz sayar.
        if (string.IsNullOrWhiteSpace(secretKey))
            return false;

        var handler = new JwtSecurityTokenHandler();

        // JWT formatı bozuksa validation denemeden false döner.
        if (!handler.CanReadToken(token))
            return false;

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            handler.ValidateToken(token, validationParameters, out var validatedToken);
            jwt = validatedToken as JwtSecurityToken;
            return jwt is not null;
        }
        catch
        {
            // İmza, süre, issuer veya audience hatasında token geçersiz kabul edilir.
            return false;
        }
    }

    // Session'daki JWT token'in icindeki rolleri okuyup verilen rol bilgisini iceriyor mu kontrol eder.
    // IConfiguration parametresi imza dogrulamasini IsAuthenticated uzerinden tetiklemek icin gerekli;
    // boylece manipule edilmis "Admin" claim'i ile yetki kazanilamaz.
    public static bool IsInRole(this ISession session, IConfiguration configuration, string roleName)
    {
        // Once tam JWT validation yapan IsAuthenticated calistirilir (imza dahil).
        // Bu cagri suresi dolmus veya imzasi bozuk token icin session'i da temizler.
        if (!session.IsAuthenticated(configuration))
            return false;

        var token = session.GetString(JwtSessionKey)!;
        var handler = new JwtSecurityTokenHandler();

        // IsAuthenticated icindeki TryValidateJwtToken zaten gecti; burada sadece claim okumak guvenli.
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
