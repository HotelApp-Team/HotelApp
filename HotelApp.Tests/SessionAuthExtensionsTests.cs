using HotelApp.Web.Helpers;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelApp.Tests;

public class SessionAuthExtensionsTests
{
    [Fact]
    public void IsAuthenticated_TokenYoksa_FalseDoner()
    {
        // Arrange: Session'da token yok, yani kullanici giris yapmamis.
        var session = new TestSession();

        // Act
        var result = session.IsAuthenticated();

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
        var result = session.IsAuthenticated();

        // Assert
        Assert.False(result);
        Assert.Null(session.GetString("JWToken"));
        Assert.Null(session.GetString("UserEmail"));
        Assert.Null(session.GetString("UserFullName"));
    }

    [Fact]
    public void IsAuthenticated_SuresiDolmusTokenVarsa_FalseDonerVeAuthBilgileriniTemizler()
    {
        // Arrange: Token JWT formatinda dogru ama suresi gecmis.
        var session = new TestSession();
        session.SetString("JWToken", CreateJwtToken(DateTime.UtcNow.AddMinutes(-5)));
        session.SetString("UserEmail", "test@hotelapp.com");
        session.SetString("UserFullName", "Test User");

        // Act
        var result = session.IsAuthenticated();

        // Assert
        Assert.False(result);
        Assert.Null(session.GetString("JWToken"));
        Assert.Null(session.GetString("UserEmail"));
        Assert.Null(session.GetString("UserFullName"));
    }

    [Fact]
    public void IsAuthenticated_GecerliTokenVarsa_TrueDonerVeSessionBilgileriKalir()
    {
        // Arrange: Token JWT formatinda dogru ve suresi henuz dolmamis.
        var session = new TestSession();
        var token = CreateJwtToken(DateTime.UtcNow.AddMinutes(30));
        session.SetString("JWToken", token);
        session.SetString("UserEmail", "test@hotelapp.com");
        session.SetString("UserFullName", "Test User");

        // Act
        var result = session.IsAuthenticated();

        // Assert
        Assert.True(result);
        Assert.Equal(token, session.GetString("JWToken"));
        Assert.Equal("test@hotelapp.com", session.GetString("UserEmail"));
        Assert.Equal("Test User", session.GetString("UserFullName"));
    }

    private static string CreateJwtToken(DateTime expires)
    {
        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@hotelapp.com")
            },
            expires: expires);

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
