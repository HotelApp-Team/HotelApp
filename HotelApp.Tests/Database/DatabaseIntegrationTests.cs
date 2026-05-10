using Microsoft.Data.SqlClient;

namespace HotelApp.Tests.Database;

[Collection(SqlServerDatabaseCollection.CollectionName)]
public class DatabaseIntegrationTests
{
    private readonly SqlServerDatabaseFixture _database;

    public DatabaseIntegrationTests(SqlServerDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task SeedData_RolesHotelsRoomTypesAmenities_Eklenir()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        Assert.True(await _database.ExecuteScalarIntAsync("SELECT COUNT(*) FROM Roles") >= 2);
        Assert.True(await _database.ExecuteScalarIntAsync("SELECT COUNT(*) FROM Hotels") >= 2);
        Assert.True(await _database.ExecuteScalarIntAsync("SELECT COUNT(*) FROM RoomTypes") >= 3);
        Assert.True(await _database.ExecuteScalarIntAsync("SELECT COUNT(*) FROM Amenities") >= 3);
        Assert.True(await _database.ExecuteScalarIntAsync("SELECT COUNT(*) FROM HotelImages") >= 3);
        Assert.True(await _database.ExecuteScalarIntAsync("SELECT COUNT(*) FROM HotelAmenities") >= 4);
    }

    [Fact]
    public async Task HotelImages_HerSeedOtelIcinAnaGorselBulunur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var mainImageCount = await _database.ExecuteScalarIntAsync(
            "SELECT COUNT(*) FROM HotelImages WHERE HotelId = @HotelId AND IsMain = 1",
            parameters => parameters.AddWithValue("@HotelId", 1));

        Assert.Equal(1, mainImageCount);
    }

    [Fact]
    public async Task HotelAmenities_OteleAtananOlanaklariTutar()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var amenityCount = await _database.ExecuteScalarIntAsync(
            "SELECT COUNT(*) FROM HotelAmenities WHERE HotelId = @HotelId",
            parameters => parameters.AddWithValue("@HotelId", 1));

        Assert.Equal(2, amenityCount);
    }

    [Fact]
    public async Task View_RoomDetails_AktifOdalariDondurur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var count = await _database.ExecuteScalarIntAsync("SELECT COUNT(*) FROM vw_RoomDetails WHERE City = @City", parameters =>
        {
            parameters.AddWithValue("@City", "Istanbul");
        });

        Assert.Equal(3, count);
    }

    [Fact]
    public async Task SpSearchAvailableRooms_KisiSayisinaGoreUygunOdalariDondurur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var result = await SearchAvailableRoomsAsync(
            city: "Istanbul",
            checkIn: new DateTime(2026, 7, 1),
            checkOut: new DateTime(2026, 7, 5),
            guestCount: 2);

        Assert.NotEmpty(result);
        Assert.All(result, room => Assert.True(room.MaxGuests >= 2));
        Assert.DoesNotContain(result, room => room.RoomTypeName == "Single");
    }

    [Fact]
    public async Task SpSearchAvailableRooms_OdaTamDolduysaSonuctanCikarir()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        // HotelRoomId 2 = Istanbul Double, TotalRooms = 2.
        // Ayni tarih araligina 2 rezervasyon ekleyince bu oda tipi artik musait gorunmemeli.
        await _database.ExecuteNonQueryAsync(
            """
            INSERT INTO Reservations (UserId, HotelRoomId, CheckInDate, CheckOutDate, GuestCount, TotalPrice, PaymentMethod, Status)
            VALUES
            (2, 2, '2026-08-01', '2026-08-05', 2, 800, 'CreditCard', 'Confirmed'),
            (2, 2, '2026-08-01', '2026-08-05', 2, 800, 'CreditCard', 'Confirmed');
            """);

        var result = await SearchAvailableRoomsAsync(
            city: "Istanbul",
            checkIn: new DateTime(2026, 8, 1),
            checkOut: new DateTime(2026, 8, 5),
            guestCount: 2);

        Assert.DoesNotContain(result, room => room.HotelRoomId == 2);
    }

    [Fact]
    public async Task SpMakeReservation_MusaitOdaVarsaRezervasyonOlustururVeToplamFiyatHesaplar()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var reservation = await MakeReservationAsync(
            userId: 2,
            hotelRoomId: 1,
            checkIn: new DateTime(2026, 9, 1),
            checkOut: new DateTime(2026, 9, 4),
            guestCount: 1);

        Assert.True(reservation.ReservationId > 0);
        Assert.Equal("Rezervasyon basariyla olusturuldu.", reservation.Message);

        var totalPrice = await _database.ExecuteScalarDecimalAsync(
            "SELECT TotalPrice FROM Reservations WHERE Id = @ReservationId",
            parameters => parameters.AddWithValue("@ReservationId", reservation.ReservationId));

        Assert.Equal(300m, totalPrice);
    }

    [Fact]
    public async Task SpMakeReservation_MisafirSayisiKapasiteyiAsarsaRezervasyonOlusturmaz()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var reservation = await MakeReservationAsync(
            userId: 2,
            hotelRoomId: 1,
            checkIn: new DateTime(2026, 10, 1),
            checkOut: new DateTime(2026, 10, 3),
            guestCount: 2);

        Assert.Equal(0, reservation.ReservationId);
        Assert.Equal("Misafir sayisi oda kapasitesini asiyor.", reservation.Message);
    }

    [Fact]
    public async Task ViewReservationDetails_RezervasyonOtelOdaVeKullaniciBilgisiniBirlestirir()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        var reservation = await MakeReservationAsync(
            userId: 2,
            hotelRoomId: 3,
            checkIn: new DateTime(2026, 11, 1),
            checkOut: new DateTime(2026, 11, 3),
            guestCount: 4);

        var count = await _database.ExecuteScalarIntAsync(
            """
            SELECT COUNT(*)
            FROM vw_ReservationDetails
            WHERE ReservationId = @ReservationId
              AND Email = 'user@hotelapp.test'
              AND HotelName = 'CI Istanbul Hotel'
              AND RoomTypeName = 'Family'
            """,
            parameters => parameters.AddWithValue("@ReservationId", reservation.ReservationId));

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Trigger_UpdateHotelRating_ReviewEkleninceOrtalamaPuaniGunceller()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        // Test kendi otelini ve kullanicisini olusturur; seed yorumlardan etkilenmez.
        var hotelId = await _database.ExecuteScalarIntAsync(
            """
            INSERT INTO Hotels (Name, Description, Address, City, Country, StarRating, Phone, Email)
            VALUES (@HotelName, 'Trigger test hotel', 'Trigger Test Address', 'TestCity', 'Turkiye', 5, '5554444444', 'trigger-hotel@hotelapp.test');

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """,
            parameters => parameters.AddWithValue("@HotelName", $"CI Review Trigger Hotel {Guid.NewGuid():N}"));

        var userId = await _database.ExecuteScalarIntAsync(
            """
            INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Phone, IsActive, IsEmailVerified)
            VALUES ('Review', 'User', @Email, 'test-hash', '5553333333', 1, 1);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """,
            parameters => parameters.AddWithValue("@Email", $"review-user-{Guid.NewGuid():N}@hotelapp.test"));

        await _database.ExecuteNonQueryAsync(
            """
            INSERT INTO Reviews (HotelId, UserId, Rating, Comment)
            VALUES (@HotelId, @UserId, 4, 'Clean and comfortable.');
            """,
            parameters =>
            {
                parameters.AddWithValue("@HotelId", hotelId);
                parameters.AddWithValue("@UserId", userId);
            });

        var avgRating = await _database.ExecuteScalarDecimalAsync(
            "SELECT AvgRating FROM Hotels WHERE Id = @HotelId",
            parameters => parameters.AddWithValue("@HotelId", hotelId));

        Assert.Equal(4.0m, avgRating);
    }

    private async Task<List<AvailableRoomRow>> SearchAvailableRoomsAsync(
        string city,
        DateTime checkIn,
        DateTime checkOut,
        int guestCount,
        int? hotelId = null)
    {
        await using var connection = new SqlConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand("sp_SearchAvailableRooms", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@HotelId", hotelId.HasValue ? hotelId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@City", city);
        command.Parameters.AddWithValue("@CheckInDate", checkIn.Date);
        command.Parameters.AddWithValue("@CheckOutDate", checkOut.Date);
        command.Parameters.AddWithValue("@GuestCount", guestCount);

        var rooms = new List<AvailableRoomRow>();

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            rooms.Add(new AvailableRoomRow
            {
                HotelRoomId = reader.GetInt32(reader.GetOrdinal("HotelRoomId")),
                RoomTypeName = reader.GetString(reader.GetOrdinal("RoomTypeName")),
                MaxGuests = reader.GetInt32(reader.GetOrdinal("MaxGuests")),
                AvailableRooms = reader.GetInt32(reader.GetOrdinal("AvailableRooms"))
            });
        }

        return rooms;
    }

    private async Task<ReservationProcedureResult> MakeReservationAsync(
        int userId,
        int hotelRoomId,
        DateTime checkIn,
        DateTime checkOut,
        int guestCount)
    {
        await using var connection = new SqlConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand("sp_MakeReservation", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@HotelRoomId", hotelRoomId);
        command.Parameters.AddWithValue("@CheckInDate", checkIn.Date);
        command.Parameters.AddWithValue("@CheckOutDate", checkOut.Date);
        command.Parameters.AddWithValue("@GuestCount", guestCount);
        command.Parameters.AddWithValue("@PaymentMethod", "CreditCard");

        var reservationId = new SqlParameter("@ReservationId", System.Data.SqlDbType.Int)
        {
            Direction = System.Data.ParameterDirection.Output
        };
        var resultMessage = new SqlParameter("@ResultMessage", System.Data.SqlDbType.NVarChar, 200)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        command.Parameters.Add(reservationId);
        command.Parameters.Add(resultMessage);

        await command.ExecuteNonQueryAsync();

        return new ReservationProcedureResult(
            Convert.ToInt32(reservationId.Value),
            Convert.ToString(resultMessage.Value) ?? string.Empty);
    }

    private sealed class AvailableRoomRow
    {
        public int HotelRoomId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public int MaxGuests { get; set; }
        public int AvailableRooms { get; set; }
    }

    private sealed record ReservationProcedureResult(int ReservationId, string Message);
}
