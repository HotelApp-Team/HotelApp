using HotelApp.Domain.DTOs;
using HotelApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HotelApp.Tests.Database;

[Collection(SqlServerDatabaseCollection.CollectionName)]
public class AuthApiIntegrationTests
{
    private readonly SqlServerDatabaseFixture _database;

    public AuthApiIntegrationTests(SqlServerDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Register_GecerliBilgilerle_KullaniciOlustururVeTokenDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        var email = CreateUniqueEmail("register");

        var response = await client.PostAsJsonAsync("/api/auth/register", new UserRegisterDto
        {
            FirstName = "Register",
            LastName = "User",
            Email = email,
            Password = "Password123!",
            Phone = "5554443322"
        });

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();

        Assert.NotNull(body);
        Assert.False(body!.RequiresEmailVerification);
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
        Assert.Equal(email, body.Email);
        Assert.Equal(1, await CountUsersByEmailAsync(email));
    }

    [Fact]
    public async Task Login_DogruBilgilerle_TokenDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        var email = CreateUniqueEmail("login");
        await RegisterUserAsync(client, email, "Password123!");

        var response = await client.PostAsJsonAsync("/api/auth/login", new UserLoginDto
        {
            Email = email,
            Password = "Password123!",
            RememberMe = false
        });

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        Assert.Equal(email, body.Email);
        Assert.True(body.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_HataliSifreyle_UnauthorizedDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        var email = CreateUniqueEmail("wrong-password");
        await RegisterUserAsync(client, email, "Password123!");

        var response = await client.PostAsJsonAsync("/api/auth/login", new UserLoginDto
        {
            Email = email,
            Password = "WrongPassword123!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Geçersiz email veya parola", json);
    }

    [Fact]
    public async Task Profile_TokenYoksa_UnauthorizedDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);

        var response = await client.GetAsync("/api/auth/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Profile_GecerliTokenIle_KullaniciBilgisiniDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        var email = CreateUniqueEmail("profile");
        var registerResponse = await RegisterUserAsync(client, email, "Password123!");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", registerResponse.Token);

        var response = await client.GetAsync("/api/auth/profile");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<UserDto>();

        Assert.NotNull(body);
        Assert.Equal(email, body!.Email);
        Assert.Contains("User", body.Roles);
    }

    [Fact]
    public async Task VerifyEmail_GecerliTokenle_EmailiDogrularVeTokenDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        var email = CreateUniqueEmail("verify");
        var verificationToken = Guid.NewGuid().ToString();
        await InsertUnverifiedUserAsync(email, verificationToken);

        var response = await client.GetAsync($"/api/auth/verify-email?token={Uri.EscapeDataString(verificationToken)}");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        Assert.Equal(email, body.Email);
        Assert.Equal(1, await CountVerifiedUsersByEmailAsync(email));
        Assert.True(string.IsNullOrEmpty(await GetVerificationTokenAsync(email)));
    }

    [Fact]
    public async Task ResendVerification_DogrulanmamisKullaniciIcin_TokeniYeniler()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        var email = CreateUniqueEmail("resend");
        var oldToken = Guid.NewGuid().ToString();
        await InsertUnverifiedUserAsync(email, oldToken);

        var response = await client.PostAsJsonAsync("/api/auth/resend-verification", new ResendVerificationDto
        {
            Email = email
        });

        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        Assert.Contains("Eğer bu email adresi kayıtlı", responseJson);

        var newToken = await GetVerificationTokenAsync(email);
        Assert.False(string.IsNullOrWhiteSpace(newToken));
        Assert.NotEqual(oldToken, newToken);
    }

    private WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = _database.ConnectionString,
                        ["Jwt:Issuer"] = "HotelApp.API",
                        ["Jwt:Audience"] = "HotelApp.Client",
                        ["Jwt:SecretKey"] = "BuCokGizliBirAnahtarOlmaliEnAz32Karakter12345",
                        ["Jwt:ExpirationMinutes"] = "60",
                        ["Jwt:RememberMeTokenExpiryDays"] = "7",
                        ["AuthSettings:UseEmailVerification"] = "false",
                        ["EmailSettings:SmtpServer"] = "localhost",
                        ["EmailSettings:SmtpPort"] = "25",
                        ["EmailSettings:SenderEmail"] = "noreply@hotelapp.test",
                        ["EmailSettings:SenderName"] = "HotelApp Test",
                        ["EmailSettings:Username"] = "test",
                        ["EmailSettings:Password"] = "test",
                        ["EmailSettings:VerifyEmailBaseUrl"] = "https://localhost/api/auth/verify-email"
                    });
                });
            });
    }

    private HttpClient CreateClient(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        // Program.cs LoggerService'i appsettings ile baslatabiliyor.
        // Test HTTP isteklerinden once singleton logger'i test DB connection string'i ile yeniden ayarliyoruz.
        LoggerService.Initialize(_database.ConnectionString);

        return client;
    }

    private static string CreateUniqueEmail(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}@hotelapp.test";

    private static async Task<LoginResponseDto> RegisterUserAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new UserRegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = password,
            Phone = "5554443322"
        });

        response.EnsureSuccessStatusCode();
        var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();

        Assert.NotNull(registerResponse);
        Assert.False(string.IsNullOrWhiteSpace(registerResponse!.Token));

        return new LoginResponseDto
        {
            Token = registerResponse.Token!,
            Email = registerResponse.Email!,
            FullName = registerResponse.FullName!,
            ExpiresAt = registerResponse.ExpiresAt!.Value
        };
    }

    private async Task InsertUnverifiedUserAsync(string email, string token)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

        await _database.ExecuteNonQueryAsync(
            """
            INSERT INTO Users
                (FirstName, LastName, Email, PasswordHash, Phone, IsActive, IsEmailVerified, EmailVerificationToken, VerificationTokenExpiry)
            VALUES
                ('Verify', 'User', @Email, @PasswordHash, '5555556677', 1, 0, @Token, DATEADD(hour, 24, SYSUTCDATETIME()));

            DECLARE @UserId INT = SCOPE_IDENTITY();
            DECLARE @RoleId INT = (SELECT Id FROM Roles WHERE Name = 'User');

            INSERT INTO UserRoles (UserId, RoleId)
            VALUES (@UserId, @RoleId);
            """,
            parameters =>
            {
                parameters.AddWithValue("@Email", email);
                parameters.AddWithValue("@PasswordHash", passwordHash);
                parameters.AddWithValue("@Token", token);
            });
    }

    private Task<int> CountUsersByEmailAsync(string email)
    {
        return _database.ExecuteScalarIntAsync(
            "SELECT COUNT(*) FROM Users WHERE Email = @Email",
            parameters => parameters.AddWithValue("@Email", email));
    }

    private Task<int> CountVerifiedUsersByEmailAsync(string email)
    {
        return _database.ExecuteScalarIntAsync(
            "SELECT COUNT(*) FROM Users WHERE Email = @Email AND IsEmailVerified = 1",
            parameters => parameters.AddWithValue("@Email", email));
    }

    private async Task<string?> GetVerificationTokenAsync(string email)
    {
        await using var connection = new SqlConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand("SELECT EmailVerificationToken FROM Users WHERE Email = @Email", connection);
        command.Parameters.AddWithValue("@Email", email);

        var result = await command.ExecuteScalarAsync();
        return result == DBNull.Value ? null : Convert.ToString(result, System.Globalization.CultureInfo.InvariantCulture);
    }
}
