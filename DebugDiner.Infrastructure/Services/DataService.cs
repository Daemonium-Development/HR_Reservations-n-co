using System.Reflection;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Configurations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Serilog;

namespace DebugDiner.Infrastructure.Services;

public class DataService(IOptions<DatabaseOptions> options, ILogger logger) : IDataService
{
    private readonly DatabaseOptions _options = options.Value;
    private readonly ILogger _logger = logger;
    
    private SqliteConnection? _connection;
    public ServiceStatus Status { get; private set; } = ServiceStatus.Stopped;

    public async Task<SqliteConnection?> StartAsync()
    {
        var dbPath = _options.ResolvedSource();
        _logger.Information("Database service starting. Path={Path}", dbPath);
        
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(dbPath)!))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            }

            _connection = new SqliteConnection($"Data Source={dbPath}");
            await _connection.OpenAsync().ConfigureAwait(false);

            await using (var pragma = _connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;";
                await pragma.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            await RunSchemaAsync().ConfigureAwait(false);
            _logger.Information("Database service started. Schema applied.");
        }
        catch (Exception ex)
        {
            Status = ServiceStatus.Error;
            _logger.Error(ex, "Database service failed to start.");
            return null;
        }
        finally
        {
            Status = ServiceStatus.Running;
        }

        return _connection;
    }

    public async Task StopAsync()
    {
        _logger.Information("Database service stopping.");
        try
        {
            if (_connection is not null)
            {
                await _connection.CloseAsync().ConfigureAwait(false);
                await _connection.DisposeAsync().ConfigureAwait(false);
                _connection = null;
            }

            Status = ServiceStatus.Stopped;
            _logger.Information("Database service stopped.");
        }
        catch (Exception ex)
        {
            Status = ServiceStatus.Error;
            _logger.Error(ex, "Database service failed to stop.");
        }
    }

    public async Task<SqliteConnection?> RestartAsync()
    {
        _logger.Information("Database service restarting.");
        await StopAsync();
        return await StartAsync();
    }

    public async Task RefreshAsync()
    {
        await RunSchemaAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task RunSchemaAsync()
    {
        await using (var pragma = _connection!.CreateCommand())
        {
            pragma.CommandText = "PRAGMA foreign_keys = OFF;";
            await pragma.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        try
        {
            string[] files = [
                "User.sql",
                "Arrangement.sql",
                "Dish.sql",
                "Reservation.sql",
                "Table.sql",
                "ArrangementDish.sql",
                "ReservationArrangement.sql"
            ];
            foreach (var file in files)
            {
                var sql = ReadEmbeddedSql(file);
                await using var cmd = _connection!.CreateCommand();
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            await using var pragma = _connection!.CreateCommand();
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