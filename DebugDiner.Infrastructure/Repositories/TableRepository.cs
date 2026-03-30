using DebugDiner.Domain.Abstractions;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class TableRepository(ILogger logger) : BaseRepository, ITableRepository
{
    public async Task<TableEntity?> GetById(int id)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }

        var query = "SELECT id, capacity, table_type FROM `table` WHERE id=@id";
        var command = Connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        var reader = await command.ExecuteReaderAsync();
        TableEntity? table = null;

        if (reader.Read())
        {
            table = new TableEntity
            {
                Id = reader.GetInt32(0),
                Capacity = reader.GetInt32(1),
                Type = MapToEnum<TableType>(reader.GetString(2)),
                CreatedAt = reader.GetDateTime(3),
                UpdatedAt = reader.GetDateTime(4)
            };
        }

        await reader.CloseAsync();

        return table;
    }

    public async Task<IEnumerable<TableEntity>> GetAll()
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var query = "SELECT id, capacity, table_type FROM `table`";
        var command = Connection.CreateCommand();
        command.CommandText = query;

        var reader = await command.ExecuteReaderAsync();
        List<TableEntity> tables = [];

        while (reader.Read())
        {
            tables.Add(new TableEntity
            {
                Id = reader.GetInt32(0),
                Capacity = reader.GetInt32(1),
                Type = MapToEnum<TableType>(reader.GetString(2)),
                CreatedAt = reader.GetDateTime(3),
                UpdatedAt = reader.GetDateTime(4)
            });
        }

        await reader.CloseAsync();

        logger.Information("Retrieved {Count} table(s)", tables.Count);
        return tables;
    }

    public async Task<IEnumerable<TableEntity>> Create(IEnumerable<TableEntity> tables)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var createdTables = new List<TableEntity>();

        foreach (var table in tables)
        {
            var query = "INSERT INTO `table` (capacity, table_type) VALUES (@capacity, @type);";
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@capacity", table.Capacity);
            command.Parameters.AddWithValue("@type", table.Type.ToString());

            var tableId = Convert.ToInt32(await command.ExecuteScalarAsync());

            query = "SELECT id, capacity, table_type FROM table WHERE id=@id";
            command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", tableId);

            var reader = await command.ExecuteReaderAsync();
            if (reader.Read())
            {
                createdTables.Add(new TableEntity
                {
                    Id = reader.GetInt32(0),
                    Capacity = reader.GetInt32(1),
                    Type = MapToEnum<TableType>(reader.GetString(2)),
                    CreatedAt = reader.GetDateTime(3),
                    UpdatedAt = reader.GetDateTime(4)
                });
            }

            await reader.CloseAsync();
        }

        logger.Debug("create {Count} table(s).", createdTables.Count);
        return createdTables;
    }

    public async Task<IEnumerable<TableEntity>> Update(IEnumerable<TableEntity> tables)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        
        var query = @"INSERT OR REPLACE INTO table
                          SET capacity=@capacity, table_type=@type
                          WHERE id=@id";
        foreach (var table in tables.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;

            command.Parameters.AddWithValue("@id", table.Id);
            command.Parameters.AddWithValue("@capacity", table.Capacity);
            command.Parameters.AddWithValue("@type", table.Type.ToString());

            await command.ExecuteNonQueryAsync();
        }

        logger.Debug("update {Count} table(s)", tables.Count());
        return tables;
    }

    public async Task Delete(IEnumerable<TableEntity> tables)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null");
            return;
        }

        foreach (var table in tables)
        {
            var query = "DELETE FROM table WHERE id=@id";
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", table.Id);

            await command.ExecuteNonQueryAsync();
        }

        logger.Information("deleted {Count} table(s)", tables.Count());
    }
}