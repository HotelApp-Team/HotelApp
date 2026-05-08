using HotelApp.API.Middleware;
using HotelApp.Domain.DTOs;
using HotelApp.Domain.Services;
using HotelApp.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Text.Json;

namespace HotelApp.Tests.Database;

[Collection(SqlServerDatabaseCollection.CollectionName)]
public class LoggerIntegrationTests
{
    private readonly SqlServerDatabaseFixture _database;

    public LoggerIntegrationTests(SqlServerDatabaseFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task LoggerService_LogInfoAsync_DbLogKaydiOlusturur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        LoggerService.Initialize(_database.ConnectionString);
        var message = $"Info log test {Guid.NewGuid():N}";

        await LoggerService.Instance.LogInfoAsync(
            message: message,
            userId: 2,
            userEmail: "user@hotelapp.test",
            controller: "LoggerIntegrationTests",
            action: "LogInfoAsync",
            ipAddress: "127.0.0.1");

        var count = await _database.ExecuteScalarIntAsync(
            "SELECT COUNT(*) FROM Logs WHERE [Level] = 'INFO' AND [Message] = @Message AND UserId = 2",
            parameters => parameters.AddWithValue("@Message", message));

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task LoggerService_LogWarningAsync_WarningSeviyesindeKayitOlusturur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        LoggerService.Initialize(_database.ConnectionString);
        var message = $"Warning log test {Guid.NewGuid():N}";

        await LoggerService.Instance.LogWarningAsync(
            message: message,
            userEmail: "warning@hotelapp.test",
            controller: "LoggerIntegrationTests",
            action: "LogWarningAsync");

        var count = await _database.ExecuteScalarIntAsync(
            "SELECT COUNT(*) FROM Logs WHERE [Level] = 'WARNING' AND [Message] = @Message",
            parameters => parameters.AddWithValue("@Message", message));

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task LoggerService_LogErrorAsync_UzunMesajVeExceptionDetailAlanlariniKisaltir()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        LoggerService.Initialize(_database.ConnectionString);
        var longMessage = new string('M', 2100);
        var longException = new string('E', 4100);

        await LoggerService.Instance.LogErrorAsync(
            message: longMessage,
            controller: "LoggerIntegrationTests",
            action: "LogErrorAsync",
            exceptionDetail: longException);

        var lengths = await GetLatestErrorLogLengthsAsync();

        Assert.Equal(2000, lengths.MessageLength);
        Assert.Equal(4000, lengths.ExceptionDetailLength);
    }

    [Fact]
    public async Task LoggerService_GetLogsAsync_SayfalamaParametreleriniGuvenliAraligaCeker()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        LoggerService.Initialize(_database.ConnectionString);

        await LoggerService.Instance.LogInfoAsync($"Paging log 1 {Guid.NewGuid():N}");
        await LoggerService.Instance.LogInfoAsync($"Paging log 2 {Guid.NewGuid():N}");

        var logs = (await LoggerService.Instance.GetLogsAsync(new LogFilterDto { PageNumber = 0, PageSize = 999 })).ToList();

        Assert.NotEmpty(logs);
        Assert.True(logs.Count <= 100);
    }

    [Fact]
    public async Task LoggerService_GetLogCountAsync_ToplamLogSayisiniDondurur()
    {
        if (!SqlServerDatabaseFixture.Enabled)
            return;

        LoggerService.Initialize(_database.ConnectionString);
        var before = await LoggerService.Instance.GetLogCountAsync(new LogFilterDto());

        await LoggerService.Instance.LogInfoAsync($"Count log test {Guid.NewGuid():N}");

        var after = await LoggerService.Instance.GetLogCountAsync(new LogFilterDto());

        Assert.True(after >= before + 1);
    }

    [Fact]
    public async Task GlobalExceptionHandler_GenericExceptionIcin_500DonerVeLogYazar()
    {
        var logger = new CapturingLoggerService();
        var handler = new GlobalExceptionHandler(logger);
        var context = CreateHttpContextWithBody();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";

        var handled = await handler.TryHandleAsync(
            context,
            new Exception("Beklenmeyen hata detayi"),
            CancellationToken.None);

        var response = await ReadJsonResponseAsync(context);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Contains("Sunucu hatası oluştu", response);
        Assert.Single(logger.ErrorLogs);
        Assert.Equal("Beklenmeyen hata detayi", logger.ErrorLogs[0].Message);
    }

    [Fact]
    public async Task GlobalExceptionHandler_InvalidOperationExceptionIcin_409DonerVeLogYazar()
    {
        var logger = new CapturingLoggerService();
        var handler = new GlobalExceptionHandler(logger);
        var context = CreateHttpContextWithBody();
        context.Request.Method = "POST";
        context.Request.Path = "/api/test";

        await handler.TryHandleAsync(
            context,
            new InvalidOperationException("Is kurali ihlali"),
            CancellationToken.None);

        var response = await ReadJsonResponseAsync(context);

        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        Assert.Contains("Is kurali ihlali", response);
        Assert.Single(logger.ErrorLogs);
    }

    [Fact]
    public async Task GlobalExceptionHandler_ArgumentExceptionIcin_400DonerVeLogYazmaz()
    {
        var logger = new CapturingLoggerService();
        var handler = new GlobalExceptionHandler(logger);
        var context = CreateHttpContextWithBody();

        await handler.TryHandleAsync(
            context,
            new ArgumentException("Parametre gecersiz"),
            CancellationToken.None);

        var response = await ReadJsonResponseAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Contains("Parametre gecersiz", response);
        Assert.Empty(logger.ErrorLogs);
    }

    [Fact]
    public async Task GlobalExceptionHandler_LogYazarkenUserClaimBilgileriniAktarir()
    {
        var logger = new CapturingLoggerService();
        var handler = new GlobalExceptionHandler(logger);
        var context = CreateHttpContextWithBody();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "42"),
            new Claim(ClaimTypes.Email, "claim-user@hotelapp.test")
        }, "TestAuth"));

        await handler.TryHandleAsync(
            context,
            new Exception("Claim log test"),
            CancellationToken.None);

        var log = Assert.Single(logger.ErrorLogs);
        Assert.Equal(42, log.UserId);
        Assert.Equal("claim-user@hotelapp.test", log.UserEmail);
    }

    private async Task<(int MessageLength, int ExceptionDetailLength)> GetLatestErrorLogLengthsAsync()
    {
        await using var connection = new SqlConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(
            """
            SELECT TOP 1 LEN([Message]) AS MessageLength, LEN(ExceptionDetail) AS ExceptionDetailLength
            FROM Logs
            WHERE [Level] = 'ERROR'
            ORDER BY Id DESC
            """,
            connection);

        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        return (reader.GetInt32(0), reader.GetInt32(1));
    }

    private static DefaultHttpContext CreateHttpContextWithBody()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadJsonResponseAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private sealed class CapturingLoggerService : ILoggerService
    {
        public List<CapturedErrorLog> ErrorLogs { get; } = new();

        public Task LogInfoAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null) => Task.CompletedTask;

        public Task LogWarningAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null) => Task.CompletedTask;

        public Task LogErrorAsync(string message, int? userId = null, string? userEmail = null, string? controller = null, string? action = null, string? ipAddress = null, string? exceptionDetail = null)
        {
            ErrorLogs.Add(new CapturedErrorLog(message, userId, userEmail, controller, action, ipAddress, exceptionDetail));
            return Task.CompletedTask;
        }

        public Task<IEnumerable<LogDto>> GetLogsAsync(LogFilterDto filter) => Task.FromResult(Enumerable.Empty<LogDto>());

        public Task<int> GetLogCountAsync(LogFilterDto filter) => Task.FromResult(0);

        public Task<LogDto?> GetLogByIdAsync(int id) => Task.FromResult<LogDto?>(null);
    }

    private sealed record CapturedErrorLog(
        string Message,
        int? UserId,
        string? UserEmail,
        string? Controller,
        string? Action,
        string? IpAddress,
        string? ExceptionDetail);
}
