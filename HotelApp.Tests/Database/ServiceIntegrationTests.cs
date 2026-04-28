using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using HotelApp.Infrastructure;
using HotelApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Tests.Database;

[Collection(SqlServerDatabaseCollection.CollectionName)]
public class ServiceIntegrationTests
{
    private readonly SqlServerDatabaseFixture _database;

    public ServiceIntegrationTests(SqlServerDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task HotelService_GetById_AktifOteliAmenitiesIleDondurur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new HotelService(unitOfWork);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("CI Istanbul Hotel", result!.Name);
        Assert.Equal("Istanbul", result.City);
        Assert.Equal(2, result.Amenities.Count);
    }

    [Fact]
    public async Task HotelService_Search_PageSifirsa_ArgumentExceptionFirlatir()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new HotelService(unitOfWork);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.SearchAsync(new HotelSearchRequestDto { City = "Istanbul" }, page: 0, pageSize: 10));
    }

    [Fact]
    public async Task HotelService_Delete_OteliSoftDeleteYapar()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var hotelId = await InsertHotelAsync("Soft Delete Test Hotel");

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new HotelService(unitOfWork);

        await service.DeleteAsync(hotelId);

        var isActive = await _database.ExecuteScalarIntAsync(
            "SELECT CAST(IsActive AS INT) FROM Hotels WHERE Id = @HotelId",
            parameters => parameters.AddWithValue("@HotelId", hotelId));

        Assert.Equal(0, isActive);
    }

    [Fact]
    public async Task RoomTypeService_AddRoomType_YeniOdaTipiEkler()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new RoomTypeService(unitOfWork);
        var name = $"Service Room {Guid.NewGuid():N}";

        await service.AddRoomTypeAsync(new CreateRoomTypeDto
        {
            Name = name,
            Description = "Service test room type",
            MaxGuests = 2
        });

        var added = await service.GetRoomTypeByNameAsync(name);

        Assert.NotNull(added);
        Assert.Equal(2, added!.MaxGuests);
    }

    [Fact]
    public async Task RoomTypeService_AddRoomType_DuplicateNameVarsa_InvalidOperationExceptionFirlatir()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new RoomTypeService(unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AddRoomTypeAsync(new CreateRoomTypeDto
            {
                Name = "Single",
                Description = "Duplicate room type",
                MaxGuests = 1
            }));
    }

    [Fact]
    public async Task RoomTypeService_Delete_KullanimdakiOdaTipiSilinemez()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new RoomTypeService(unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteRoomTypeAsync(1));
    }

    [Fact]
    public async Task HotelRoomService_GetAvailableRooms_SpSonucunuDondurur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new HotelRoomService(unitOfWork);

        var result = await service.GetAvailableRoomsAsync(
            hotelId: 1,
            city: "Istanbul",
            checkIn: new DateTime(2026, 12, 1),
            checkOut: new DateTime(2026, 12, 5),
            guestCount: 2);

        var rooms = result.ToList();
        Assert.NotEmpty(rooms);
        Assert.All(rooms, room => Assert.True(room.MaxGuests >= 2));
    }

    [Fact]
    public async Task HotelRoomService_AddRooms_DuplicateHotelRoomVarsa_InvalidOperationExceptionFirlatir()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new HotelRoomService(unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AddRoomsAsync(new CreateHotelRoomDto
            {
                HotelId = 1,
                RoomTypeId = 1,
                PricePerNight = 150,
                TotalRooms = 2
            }));
    }

    [Fact]
    public async Task AmenityService_Delete_KullanimdakiOlanakSilinemez()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new AmenityService(unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(1));
    }

    [Fact]
    public async Task AmenityService_AddAmenitiesToHotel_EklenenVeBulunamayanSayilariniDondurur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new AmenityService(unitOfWork);

        var result = await service.AddAmenitiesToHotelAsync(2, new List<int> { 2, 999 });

        Assert.Equal(1, result.AddedCount);
        Assert.Equal(1, result.NotFoundCount);
        Assert.Contains(999, result.NotFoundIds);
    }

    [Fact]
    public async Task UserService_UpdateProfile_KullaniciProfiliniGunceller()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var userId = await InsertUserAsync("profile-service-user@hotelapp.test");

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = new UserService(unitOfWork);

        var result = await service.UpdateProfileAsync(userId, new ProfileDto
        {
            FirstName = "Updated",
            LastName = "Profile",
            Phone = "5559998877"
        });

        Assert.Equal("Updated", result.FirstName);
        Assert.Equal("Profile", result.LastName);
        Assert.Equal("5559998877", result.Phone);
    }

    [Fact]
    public async Task AuthService_Register_DuplicateEmailVarsa_InvalidOperationExceptionFirlatir()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = CreateAuthService(unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(new UserRegisterDto
            {
                FirstName = "Duplicate",
                LastName = "User",
                Email = "user@hotelapp.test",
                Password = "Password123!",
                Phone = "5552222222"
            }));
    }

    [Fact]
    public async Task AuthService_Login_GecerliKullaniciyaTokenDoner()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var email = $"auth-service-{Guid.NewGuid():N}@hotelapp.test";
        await InsertUserAsync(email, password: "Password123!");

        await using var context = CreateContext();
        using var unitOfWork = new UnitOfWork(context);
        var service = CreateAuthService(unitOfWork);

        var result = await service.LoginAsync(new UserLoginDto
        {
            Email = email,
            Password = "Password123!"
        });

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal(email, result.Email);
    }

    private HotelAppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<HotelAppDbContext>()
            .UseSqlServer(_database.ConnectionString)
            .Options;

        return new HotelAppDbContext(options);
    }

    private static AuthService CreateAuthService(UnitOfWork unitOfWork)
    {
        var jwtService = new JwtService(
            secretKey: "BuCokGizliBirAnahtarOlmaliEnAz32Karakter12345",
            issuer: "HotelApp.API",
            audience: "HotelApp.Client",
            expirationMinutes: 60,
            rememberMeTokenExpiryDays: 7);

        return new AuthService(unitOfWork, jwtService, new FakeEmailService(), useEmailVerification: false);
    }

    private async Task<int> InsertHotelAsync(string name)
    {
        await _database.ExecuteNonQueryAsync(
            """
            INSERT INTO Hotels (Name, Description, Address, City, Country, StarRating, Phone, Email)
            VALUES (@Name, 'Service test hotel', 'Service Address', 'Service City', 'Turkiye', 4, '02121112233', 'service-hotel@test.com');
            """,
            parameters => parameters.AddWithValue("@Name", name));

        return await _database.ExecuteScalarIntAsync(
            "SELECT Id FROM Hotels WHERE Name = @Name",
            parameters => parameters.AddWithValue("@Name", name));
    }

    private async Task<int> InsertUserAsync(string email, string password = "Password123!")
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        await _database.ExecuteNonQueryAsync(
            """
            INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Phone, IsActive, IsEmailVerified)
            VALUES ('Service', 'User', @Email, @PasswordHash, '5557778899', 1, 1);

            DECLARE @UserId INT = SCOPE_IDENTITY();
            DECLARE @RoleId INT = (SELECT Id FROM Roles WHERE Name = 'User');

            INSERT INTO UserRoles (UserId, RoleId)
            VALUES (@UserId, @RoleId);
            """,
            parameters =>
            {
                parameters.AddWithValue("@Email", email);
                parameters.AddWithValue("@PasswordHash", passwordHash);
            });

        return await _database.ExecuteScalarIntAsync(
            "SELECT Id FROM Users WHERE Email = @Email",
            parameters => parameters.AddWithValue("@Email", email));
    }

    private sealed class FakeEmailService : IEmailService
    {
        public Task SendVerificationEmailAsync(string toEmail, string firstName, string token) => Task.CompletedTask;

        public Task<bool> SendVerificationEmailSafelyAsync(string toEmail, string firstName, string token) => Task.FromResult(true);
    }
}
