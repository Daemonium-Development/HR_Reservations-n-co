using DebugDiner.Domain.Abstractions;
using Microsoft.Data.Sqlite;

namespace DebugDiner.Infrastructure.Repositories;

public class BaseRepository : IBaseRepository
{
    protected SqliteConnection? Connection { get; private set; }
    
    public void SetConnection(SqliteConnection connection)
    {
        if (Connection != null) throw new InvalidOperationException("Connection already set.");
        
        Connection = connection;
    }

    protected virtual T MapToEnum<T>(string value) where T : Enum => (T) Enum.Parse(typeof(T), value);
}