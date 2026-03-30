using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class TableRepository(ILogger logger) : ITableRepository
{
    public SqliteConnection? Connection { get; set; }
    
    public async Task<IEnumerable<TableEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        if (ids is null)
        {
            return await GetAll();
        }
        
        var entityList = new List<TableEntity>();
        foreach (var id in ids)
        {
            var entity = await GetById(id);
            if (entity is null) continue;
            entityList.Add(entity);
        }
        return entityList;
    }
    
    private async Task<TableEntity?> GetById(int id)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }

        var query = "SELECT * FROM `table` WHERE id=@id";
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
                Type = reader.GetString(2).MapToEnum<TableType>(),
                CreatedAt = reader.GetDateTime(3),
                UpdatedAt = reader.GetDateTime(4)
            };
        }

        await reader.CloseAsync();

        return table;
    }

    private async Task<IEnumerable<TableEntity>> GetAll()
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var query = "SELECT * FROM `table`";
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
                Type = reader.GetString(2).MapToEnum<TableType>(),
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
        var query = "INSERT INTO `table` (capacity, type, updated_at) VALUES (@capacity, @type, @updatedAt);";
        foreach (var table in tables)
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@capacity", table.Capacity);
            command.Parameters.AddWithValue("@type", table.Type.ToString());
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);

            await command.ExecuteScalarAsync();
            createdTables.Add(table);
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
        
        var query = @"INSERT OR REPLACE INTO `table` (capacity, `type`, updated_at) VALUES (@capaity, @type, @updated_at) WHERE id=@id";
        foreach (var table in tables.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            
            command.Parameters.AddWithValue("@capacity", table.Capacity);
            command.Parameters.AddWithValue("@type", table.Type.ToString());
            command.Parameters.AddWithValue("@updated_at", DateTime.Now);

            await command.ExecuteNonQueryAsync();
        }

        logger.Debug("update {Count} table(s)", tables.Count());
        return tables;
    }

    public async Task<int> Delete(IEnumerable<TableEntity> tables)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null");
            return 0;
        }

        var query = "DELETE FROM `table` WHERE id=@id";
        var deleted = 0;
        foreach (var table in tables)
        {
            try
            {
                var command = Connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@id", table.Id);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error deleting table with id {Id}", table.Id);
                continue;
            }
            deleted++;
        }

        logger.Information("deleted {Count} table(s)", deleted);
        return deleted;
    }
}