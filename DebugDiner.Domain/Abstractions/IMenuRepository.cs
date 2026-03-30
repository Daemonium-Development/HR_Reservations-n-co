namespace DebugDiner.Domain.Abstractions;

public interface IMenuRepository
{
    Task<DishEntity?> GetById(int id);
    Task<IEnumerable<DishEntity>> GetAll();
    Task<IEnumerable<DishEntity>> Create(IEnumerable<DishEntity> dishes);
    Task<IEnumerable<DishEntity>> Update(IEnumerable<DishEntity> dishes);
    Task Delete(IEnumerable<DishEntity> dishes);
}