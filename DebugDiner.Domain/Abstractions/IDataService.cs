using Microsoft.Data.Sqlite;

namespace DebugDiner.Domain.Abstractions;

public interface IDataService : IDisposable
{
    public SqliteConnection? Connection { get; }
    public ServiceStatus Status { get; }
    public Task<SqliteConnection?> StartAsync();
    public Task StopAsync();
    public Task<SqliteConnection?> RestartAsync();
    public Task RefreshAsync();
}
