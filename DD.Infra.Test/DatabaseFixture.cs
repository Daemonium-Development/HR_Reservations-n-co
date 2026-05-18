using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Configurations;
using DebugDiner.Infrastructure.Repositories;
using DebugDiner.Infrastructure.Services;

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
    }

    public void Dispose()
    {
        _db?.Dispose();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
    }

    public ArrangementRepository GetArrangementRepository() => new ArrangementRepository(_logger.Object, _db);
    public MenuRepository GetMenuRepository() => new MenuRepository(_logger.Object, _db, GetArrangementRepository());
    public ReservationRepository GetReservationRepository() => new ReservationRepository(_logger.Object, _db);
    public TableRepository GetTableRepository() => new TableRepository(_logger.Object, _db);
    public UserRepository GetUserRepository() => new UserRepository(_logger.Object, _db);
}
