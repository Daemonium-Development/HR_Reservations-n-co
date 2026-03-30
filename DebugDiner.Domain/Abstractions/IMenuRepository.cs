namespace DebugDiner.Domain.Abstractions;

public interface IMenuRepository
{
    Task<IEnumerable<DishEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
    Task<IEnumerable<DishEntity>> Create(IEnumerable<DishEntity> dishes);
    Task<IEnumerable<DishEntity>> Update(IEnumerable<DishEntity> dishes);
    Task<int> Delete(IEnumerable<DishEntity> dishes);
}