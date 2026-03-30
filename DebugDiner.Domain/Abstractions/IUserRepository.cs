using Microsoft.Data.Sqlite;

namespace DebugDiner.Domain.Abstractions;

public interface IUserRepository
{
    SqliteConnection? Connection { get; set; }
    Task<IEnumerable<UserEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
    Task<IEnumerable<UserEntity>> Create(IEnumerable<UserEntity> users);
    Task<IEnumerable<UserEntity>> Update(IEnumerable<UserEntity> users);
    Task Delete(IEnumerable<UserEntity> users);
}