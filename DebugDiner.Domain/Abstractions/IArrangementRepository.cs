namespace DebugDiner.Domain.Abstractions;

public interface IArrangementRepository
{
    Task<IEnumerable<ArrangementEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
    Task<IEnumerable<ArrangementEntity>> Create(IEnumerable<ArrangementEntity> arrangements);
    Task<IEnumerable<ArrangementEntity>> Update(IEnumerable<ArrangementEntity> arrangements);
    Task<int> Delete(IEnumerable<ArrangementEntity> arrangements);
}
