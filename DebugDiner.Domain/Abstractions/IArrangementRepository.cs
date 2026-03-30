using Microsoft.Data.Sqlite;

namespace DebugDiner.Domain.Abstractions;

public interface IArrangementRepository
{
    SqliteConnection? Connection { get; set; }
    Task<IEnumerable<ArrangementEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
    Task<IEnumerable<ArrangementEntity>> Create(IEnumerable<ArrangementEntity> arrangements);
    Task<IEnumerable<ArrangementEntity>> Update(IEnumerable<ArrangementEntity> arrangements);
    Task Delete(IEnumerable<ArrangementEntity> arrangements);
}