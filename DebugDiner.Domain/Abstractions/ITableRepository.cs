namespace DebugDiner.Domain.Abstractions;

public interface ITableRepository
{
    Task<IEnumerable<TableEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
    Task<IEnumerable<TableEntity>> Create(IEnumerable<TableEntity> tables);
    Task<IEnumerable<TableEntity>> Update(IEnumerable<TableEntity> tables);
    Task<int> Delete(IEnumerable<TableEntity> tables);
}
