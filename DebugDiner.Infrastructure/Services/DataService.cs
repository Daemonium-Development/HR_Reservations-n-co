using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Configurations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Serilog;
using System.Reflection;

namespace DebugDiner.Infrastructure.Services;

public class DataService(IOptions<DatabaseOptions> options, ILogger logger) : IDataService
{
    public SqliteConnection? Connection { get; private set; }

    private readonly DatabaseOptions _options = options.Value;
    public ServiceStatus Status { get; private set; } = ServiceStatus.Stopped;

    public async Task<SqliteConnection?> StartAsync()
    {
        var dbPath = _options.ResolvedSource();
        logger.Information("Database service starting. Path={Path}", dbPath);

        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(dbPath)!))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            }

            Connection = new SqliteConnection($"Data Source={dbPath}");
            await Connection.OpenAsync().ConfigureAwait(false);

            await using (var pragma = Connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;";
                await pragma.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            await RunSchemaAsync().ConfigureAwait(false);
            logger.Information("Database service started. Schema applied.");

            Status = ServiceStatus.Running;
            return Connection;
        }
        catch (Exception ex)
        {
            Status = ServiceStatus.Error;
            logger.Error(ex, "Database service failed to start.");
            return null;
        }
    }

    public async Task StopAsync()
    {
        logger.Information("Database service stopping.");
        try
        {
            if (Connection is not null)
            {
                await Connection.CloseAsync().ConfigureAwait(false);
                await Connection.DisposeAsync().ConfigureAwait(false);
                Connection = null;
            }

            Status = ServiceStatus.Stopped;
            logger.Information("Database service stopped.");
        }
        catch (Exception ex)
        {
            Status = ServiceStatus.Error;
            logger.Error(ex, "Database service failed to stop.");
        }
    }

    public async Task<SqliteConnection?> RestartAsync()
    {
        logger.Information("Database service restarting.");
        await StopAsync();
        return await StartAsync();
    }

    public async Task RefreshAsync()
    {
        await RunSchemaAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        Connection?.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task RunSchemaAsync()
    {
        await using (var pragma = Connection!.CreateCommand())
        {
            pragma.CommandText = "PRAGMA foreign_keys = OFF;";
            await pragma.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        try
        {
            string[] files = [
                "User.sql",
                "Arrangement.sql",
                "Menu.sql",
                "Reservation.sql",
                "Table.sql",
                "ArrangementDish.sql",
                "ReservationArrangement.sql"
            ];
            foreach (var file in files)
            {
                var sql = ReadEmbeddedSql(file);
                await using var cmd = Connection!.CreateCommand();
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            await using var pragma = Connection!.CreateCommand();
            pragma.CommandText = "PRAGMA foreign_keys = ON;";
            await pragma.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    private static string ReadEmbeddedSql(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var resourceName = assembly
                               .GetManifestResourceNames()
                               .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                           ?? throw new InvalidOperationException(
                               $"Embedded SQL resource '{fileName}' not found in {assembly.GetName().Name}. " +
                               "Ensure the file is included with <EmbeddedResource> in the .csproj.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
