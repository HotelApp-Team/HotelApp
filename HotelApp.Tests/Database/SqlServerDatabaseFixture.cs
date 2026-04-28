using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace HotelApp.Tests.Database;

[CollectionDefinition(CollectionName)]
public class SqlServerDatabaseCollection : ICollectionFixture<SqlServerDatabaseFixture>
{
    public const string CollectionName = "SQL Server database tests";
}

public class SqlServerDatabaseFixture : IAsyncLifetime
{
    private const string DefaultConnectionString =
        "Server=localhost,1433;Database=HotelApp_Test;User Id=sa;Password=HotelApp_CI_Password123!;TrustServerCertificate=True;Encrypt=False";

    public static bool Enabled =>
        string.Equals(Environment.GetEnvironmentVariable("RUN_DB_TESTS"), "true", StringComparison.OrdinalIgnoreCase);

    public string ConnectionString { get; } =
        Environment.GetEnvironmentVariable("HOTELAPP_TEST_CONNECTION_STRING") ?? DefaultConnectionString;

    public async Task InitializeAsync()
    {
        if (!Enabled)
            return;

        await WaitForSqlServerAsync();
        await RecreateDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public async Task<int> ExecuteScalarIntAsync(string sql, Action<SqlParameterCollection>? addParameters = null)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        addParameters?.Invoke(command.Parameters);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<decimal> ExecuteScalarDecimalAsync(string sql, Action<SqlParameterCollection>? addParameters = null)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        addParameters?.Invoke(command.Parameters);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToDecimal(result);
    }

    public async Task ExecuteNonQueryAsync(string sql, Action<SqlParameterCollection>? addParameters = null)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        addParameters?.Invoke(command.Parameters);

        await command.ExecuteNonQueryAsync();
    }

    private async Task WaitForSqlServerAsync()
    {
        var masterConnectionString = BuildMasterConnectionString();
        Exception? lastException = null;

        for (var attempt = 1; attempt <= 60; attempt++)
        {
            try
            {
                await using var connection = new SqlConnection(masterConnectionString);
                await connection.OpenAsync();
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        throw new InvalidOperationException("SQL Server test container hazir hale gelemedi.", lastException);
    }

    private async Task RecreateDatabaseAsync()
    {
        var databaseName = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "Sql", "hotelapp_test_schema_seed.sql");
        var script = await File.ReadAllTextAsync(scriptPath);
        script = script.Replace("$(DatabaseName)", databaseName, StringComparison.OrdinalIgnoreCase);

        var batches = Regex.Split(
                script,
                @"^\s*GO\s*;?\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Select(batch => batch.Trim())
            .Where(batch => !string.IsNullOrWhiteSpace(batch));

        await using var connection = new SqlConnection(BuildMasterConnectionString());
        await connection.OpenAsync();

        foreach (var batch in batches)
        {
            await using var command = new SqlCommand(batch, connection)
            {
                CommandTimeout = 120
            };

            await command.ExecuteNonQueryAsync();
        }
    }

    private string BuildMasterConnectionString()
    {
        var builder = new SqlConnectionStringBuilder(ConnectionString)
        {
            InitialCatalog = "master"
        };

        return builder.ConnectionString;
    }
}
