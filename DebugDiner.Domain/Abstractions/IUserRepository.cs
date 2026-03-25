namespace DebugDiner.Domain.Abstractions;

public interface IUserRepository
{
    Task<UserEntity?> GetById(int id);
    Task<IEnumerable<UserEntity>> GetAll();
    Task<IEnumerable<UserEntity>> Create(IEnumerable<UserEntity> users);
    Task<IEnumerable<UserEntity>> Update(IEnumerable<UserEntity> users);
    Task Delete(IEnumerable<UserEntity> users);
}