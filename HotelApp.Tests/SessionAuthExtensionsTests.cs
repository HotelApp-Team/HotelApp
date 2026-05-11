using HotelApp.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelApp.Tests;

public class SessionAuthExtensionsTests
{
    // Testler boyunca tutarli olmasi icin sabit Jwt config; production appsettings ile birebir aynisi kullanilir.
    private const string TestSecretKey = "BuCokGizliBirAnahtarOlmaliEnAz32Karakter12345";
    private const string TestIssuer = "HotelApp.API";
    private const string TestAudience = "HotelApp.Client";

    [Fact]
    public void IsAuthenticated_TokenYoksa_FalseDoner()
    {
        // Arrange: Session'da token yok, yani kullanici giris yapmamis.
        var session = new TestSession();

        // Act
        var result = session.IsAuthenticated(BuildConfiguration());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAuthenticated_BozukTokenVarsa_FalseDonerVeAuthBilgileriniTemizler()
    {
        // Arrange: Session'da JWT formatinda olmayan bozuk bir token var.
        var session = new TestSession();
        session.SetString("JWToken", "bozuk-token");
        session.SetString("UserEmail", "test@hotelapp.com");
        session.SetString("UserFullName", "Test User");

        // Act
        var result = session.IsAuthenticated(BuildConfiguration());

        // Assert
        Assert.False(result);
        Assert.Null(session.GetString("JWToken"));
        Assert.Null(session.GetString("UserEmail"));
        Assert.Null(session.GetString("UserFullName"));
    }

    [Fact]
    public void IsAuthenticated_SuresiDolmusTokenVarsa_FalseDonerVeAuthBilgileriniTemizler()
    {
        // Arrange: Token imzasi dogru ama suresi gecmis; TryValidateJwtToken bunu reddetmeli.
        var session = new TestSession();
        session.SetString("JWToken", CreateJwtToken(DateTime.UtcNow.AddMinutes(-5)));
        session.SetString("UserEmail", "test@hotelapp.com");
        session.SetString("UserFullName", "Test User");

        // Act
        var result = session.IsAuthenticated(BuildConfiguration());

        // Assert
        Assert.False(result);
        Assert.Null(session.GetString("JWToken"));
        Assert.Null(session.GetString("UserEmail"));
        Assert.Null(session.GetString("UserFullName"));
    }

    [Fact]
    public void IsAuthenticated_ImzasiBozukTokenVarsa_FalseDonerVeAuthBilgileriniTemizler()
    {
        // Arrange: Yanlis bir secret key ile imzalanmis token. Payload "valid" gozukse de signature gecmemeli.
        // S5-02 kapsaminda eklenen kritik regression testi.
        var session = new TestSession();
        session.SetString("JWToken", CreateJwtToken(DateTime.UtcNow.AddMinutes(30), secretKey: "FarkliBirAnahtarBuCokGizliEnAz32Karakter12345"));
        session.SetString("UserEmail", "test@hotelapp.com");
        session.SetString("UserFullName", "Test User");

        // Act
        var result = session.IsAuthenticated(BuildConfiguration());

        // Assert
        Assert.False(result);
        Assert.Null(session.GetString("JWToken"));
    }

    [Fact]
    public void IsAuthenticated_GecerliTokenVarsa_TrueDonerVeSessionBilgileriKalir()
    {
        // Arrange: Dogru imzali, dogru issuer/audience'a sahip, suresi dolmamis token.
        var session = new TestSession();
        var token = CreateJwtToken(DateTime.UtcNow.AddMinutes(30));
        session.SetString("JWToken", token);
        session.SetString("UserEmail", "test@hotelapp.com");
        session.SetString("UserFullName", "Test User");

        // Act
        var result = session.IsAuthenticated(BuildConfiguration());

        // Assert
        Assert.True(result);
        Assert.Equal(token, session.GetString("JWToken"));
        Assert.Equal("test@hotelapp.com", session.GetString("UserEmail"));
        Assert.Equal("Test User", session.GetString("UserFullName"));
    }

    // appsettings'in test karsiligi: in-memory IConfiguration. Web Jwt:* anahtarlari okunabilsin diye.
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = TestSecretKey,
                ["Jwt:Issuer"] = TestIssuer,
                ["Jwt:Audience"] = TestAudience
            })
            .Build();
    }

    // Testlerde gercek API'siz JWT uretmek icin yardimci.
    // secretKey override edilebilir; "imzasi bozuk" senaryolarini test etmek icin lazim.
    private static string CreateJwtToken(DateTime expires, string? secretKey = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? TestSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@hotelapp.com")
            },
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// Testlerde gercek HTTP session yerine kullandigimiz basit in-memory session.
// Boylece IsAuthenticated() metodunu tarayici/API calistirmadan unit test edebiliyoruz.
internal class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStorage = new();

    public IEnumerable<string> Keys => _sessionStorage.Keys;
    public string Id { get; } = Guid.NewGuid().ToString();
    public bool IsAvailable => true;

    public void Clear() => _sessionStorage.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _sessionStorage.Remove(key);

    public void Set(string key, byte[] value) => _sessionStorage[key] = value;

    public bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value) => _sessionStorage.TryGetValue(key, out value);
}
