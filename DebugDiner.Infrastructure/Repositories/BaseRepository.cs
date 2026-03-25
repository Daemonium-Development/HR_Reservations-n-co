using Microsoft.Data.Sqlite;

namespace DebugDiner.Infrastructure.Repositories;

public class BaseRepository
{
    protected SqliteConnection? Connection { get; private set; }
    
    protected void SetConnection(SqliteConnection connection)
    {
        Connection = connection;
    }

    protected virtual T MapToEnum<T>(string value) where T : Enum => (T) Enum.Parse(typeof(T), value);
}