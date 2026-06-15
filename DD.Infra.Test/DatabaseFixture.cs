using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Configurations;
using DebugDiner.Infrastructure.Repositories;
using DebugDiner.Infrastructure.Services;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

using Moq;

using Serilog;

namespace DD.Infra.Test;

public class DatabaseFixture : IDisposable
{
    private readonly Mock<ILogger> _logger;
    private readonly IDataService _db;
    private readonly string _dbPath;

    public DatabaseFixture()
    {
        _logger = new Mock<ILogger>();
        _dbPath = $"test_{Guid.NewGuid():N}.db";
        var options = Options.Create(new DatabaseOptions { Source = _dbPath });
        _db = new DataService(options, _logger.Object);
        _db.StartAsync().Wait();

        var seed = _db.Connection!.CreateCommand();
        seed.CommandText =
            "INSERT INTO `user` (`name`, `email`, `password_hash`, `role`, `created_at`, `updated_at`) " +
            "VALUES ('Seed User', 'seed@example.com', 'seed-hash', 'Admin', DATETIME('now'), DATETIME('now'));";
        seed.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _db?.Dispose();
        SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        TryDelete(_dbPath);
        TryDelete(_dbPath + "-wal");
        TryDelete(_dbPath + "-shm");
        GC.SuppressFinalize(this);
    }

    private static void TryDelete(string path)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
                return;
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }
    }

    public ArrangementRepository GetArrangementRepository() => new ArrangementRepository(_logger.Object, _db);
    public MenuRepository GetMenuRepository() => new MenuRepository(_logger.Object, _db, GetArrangementRepository());
    public ReservationRepository GetReservationRepository() => new ReservationRepository(_logger.Object, _db);
    public TableRepository GetTableRepository() => new TableRepository(_logger.Object, _db);
    public UserRepository GetUserRepository() => new UserRepository(_logger.Object, _db);
}
