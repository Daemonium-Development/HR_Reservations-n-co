using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class TableRepository(ILogger logger, IDataService data) : ITableRepository
{
    public async Task<IEnumerable<TableEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        var command = data.Connection.CreateCommand();
        if (ids is null)
        {
            command.CommandText = QueryConstants.GetAll.Replace("{table}", "`table`");
        }
        else
        {
            command.CommandText = QueryConstants.GetById
                .Replace("{table}", "`table`")
                .Replace("{values}", string.Join(",", ids));
        }
        var reader = await command.ExecuteReaderAsync();

        var tables = new List<TableEntity>();
        while (reader.Read())
        {
            tables.Add(new TableEntity
            {
                Id = reader.GetInt32(0),
                Capacity = reader.GetInt32(1),
                Type = reader.GetString(2).MapToEnum<TableType>(),
                CreatedAt = DateTime.Parse(reader.GetString(3)),
                UpdatedAt = reader.IsDBNull(4) ? DateTime.Now : DateTime.Parse(reader.GetString(4))
            });
        }

        return tables;
    }

    public async Task<IEnumerable<TableEntity>> Create(IEnumerable<TableEntity> tables)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        string[] columns = ["capacity", "type"];
        string[] values = ["@capacity", "@type"];

        var ids = new List<long>();
        foreach (var table in tables.AsEnumerable())
        {
            long newId = 0;
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Insert
                    .Replace("{table}", "`table`")
                    .Replace("{columns}", string.Join(",", columns))
                    .Replace("{values}", string.Join(",", values));

                command.Parameters.AddWithValue("@capacity", table.Capacity);
                command.Parameters.AddWithValue("@type", table.Type.ToString());

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                newId = (long)result;

                logger.Debug("Table with id {Id} created.", newId);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error creating table with id {Id}", table.Id);
                continue;
            }
            ids.Add(newId);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<IEnumerable<TableEntity>> Update(IEnumerable<TableEntity> tables)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var ids = new List<long>();
        foreach (var table in tables.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Update
                    .Replace("{table}", "`table`")
                    .Replace("{columns}", string.Join(",", ["capacity = @capacity", "type = @type"]));

                command.Parameters.AddWithValue("@id", table.Id);
                command.Parameters.AddWithValue("@capacity", table.Capacity);
                command.Parameters.AddWithValue("@type", table.Type.ToString());

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                var updated = (long)result;

                logger.Debug("Table with id {Id} updated.", updated);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error updating table with id {Id}", table.Id);
                continue;
            }
            ids.Add(table.Id);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<int> Delete(IEnumerable<TableEntity> tables)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return 0;
        }

        var deleted = 0;
        foreach (var table in tables.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Delete.Replace("{table}", "`table`");

                command.Parameters.AddWithValue("@id", table.Id);
                await command.ExecuteNonQueryAsync();

                logger.Debug("Table with id {Id} deleted.", table.Id);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error deleting table with id {Id}", table.Id);
                continue;
            }
            deleted++;
        }

        return deleted;
    }
}