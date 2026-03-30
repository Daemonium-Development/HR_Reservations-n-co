using Microsoft.Data.Sqlite;

namespace DebugDiner.Domain.Abstractions;

public interface IBaseRepository
{
    void SetConnection(SqliteConnection connection);
}