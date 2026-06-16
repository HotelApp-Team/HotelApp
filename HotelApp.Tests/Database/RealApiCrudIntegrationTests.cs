using HotelApp.Domain.DTOs;
using HotelApp.Domain.Entities;
using HotelApp.Domain.Services;
using HotelApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HotelApp.Tests.Database;

[Collection(SqlServerDatabaseCollection.CollectionName)]
public class RealApiCrudIntegrationTests
{
    private readonly SqlServerDatabaseFixture _database;

    public RealApiCrudIntegrationTests(SqlServerDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Hotels_GetAll_GercekEndpoint_PagedHotelListDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);

        var response = await client.GetAsync("/api/hotels?page=1&pageSize=10");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<PagedResponseDto<HotelListDto>>();

        Assert.NotNull(body);
        Assert.True(body!.TotalCount >= 2);
        Assert.NotEmpty(body.Data);
    }

    [Fact]
    public async Task Hotels_GetById_GercekEndpoint_HotelDetailVeAmenitiesDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);

        var response = await client.GetAsync("/api/hotels/1");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<HotelDetailDto>();

        Assert.NotNull(body);
        Assert.Equal("CI Istanbul Hotel", body!.Name);
        Assert.Equal(2, body.Amenities.Count);
    }

    [Fact]
    public async Task Hotels_Create_TokensizIstek_UnauthorizedDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/hotels", CreateHotelDto("Unauthorized Hotel"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Hotels_Create_UserTokenIle_ForbiddenDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwtToken("User", userId: 2));

        var response = await client.PostAsJsonAsync("/api/hotels", CreateHotelDto("Forbidden Hotel"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Hotels_Create_AdminTokenIle_CreatedVeDbKaydiOlusturur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwtToken("Admin", userId: 1));
        var hotelName = $"API Created Hotel {Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync("/api/hotels", CreateHotelDto(hotelName));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var count = await _database.ExecuteScalarIntAsync(
            "SELECT COUNT(*) FROM Hotels WHERE Name = @Name AND IsActive = 1",
            parameters => parameters.AddWithValue("@Name", hotelName));

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Amenities_Create_AdminTokenIle_CreatedVeDbKaydiOlusturur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwtToken("Admin", userId: 1));
        var amenityName = $"API Amenity {Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync("/api/amenities", new AmenityCreateDto
        {
            Name = amenityName,
            Icon = "api-icon"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var count = await _database.ExecuteScalarIntAsync(
            "SELECT COUNT(*) FROM Amenities WHERE Name = @Name",
            parameters => parameters.AddWithValue("@Name", amenityName));

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task RoomTypes_Delete_KullanimdakiOdaTipiIcin_ConflictDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwtToken("Admin", userId: 1));

        var response = await client.DeleteAsync("/api/roomtypes/1");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task HotelRooms_Search_GecersizTarihIcin_BadRequestDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        using var factory = CreateFactory();
        using var client = CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/hotels/1/rooms/search", new SearchAvailableRoomsRequestDto
        {
            City = "Istanbul",
            CheckIn = new DateTime(2026, 6, 10),
            CheckOut = new DateTime(2026, 6, 10),
            GuestCount = 2
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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

    private static HotelCreateDto CreateHotelDto(string name)
    {
        return new HotelCreateDto
        {
            Name = name,
            Description = "API integration test hotel",
            Address = "API Test Address",
            CityId = 1,
            Country = "Turkiye",
            Phone = "02121112233",
            Email = "api-hotel@test.com",
            StarRating = 4
        };
    }

    private static string CreateJwtToken(string role, int userId)
    {
        var user = new User
        {
            Id = userId,
            FirstName = "API",
            LastName = role,
            Email = $"api-{role.ToLowerInvariant()}@hotelapp.test"
        };
        user.Roles.Add(new Role { Id = role == "Admin" ? 1 : 2, Name = role });

        var jwtService = new JwtService(
            secretKey: "BuCokGizliBirAnahtarOlmaliEnAz32Karakter12345",
            issuer: "HotelApp.API",
            audience: "HotelApp.Client",
            expirationMinutes: 60,
            rememberMeTokenExpiryDays: 7);

        return jwtService.GenerateToken(user, rememberMe: false, out _);
    }
}
