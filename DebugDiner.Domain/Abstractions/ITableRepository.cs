namespace DebugDiner.Domain.Abstractions;

public interface ITableRepository
{
    Task<TableEntity?> GetById(int id);
    Task<IEnumerable<TableEntity>> GetAll();
    Task<IEnumerable<TableEntity>> Create(IEnumerable<TableEntity> tables);
    Task<IEnumerable<TableEntity>> Update(IEnumerable<TableEntity> tables);
    Task Delete(IEnumerable<TableEntity> tables);
}