using Microsoft.Data.Sqlite;

namespace DebugDiner.Domain.Abstractions;

public interface ITableRepository
{
    SqliteConnection? Connection { get; set; }
    Task<IEnumerable<TableEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
    Task<IEnumerable<TableEntity>> Create(IEnumerable<TableEntity> tables);
    Task<IEnumerable<TableEntity>> Update(IEnumerable<TableEntity> tables);
    Task Delete(IEnumerable<TableEntity> tables);
}