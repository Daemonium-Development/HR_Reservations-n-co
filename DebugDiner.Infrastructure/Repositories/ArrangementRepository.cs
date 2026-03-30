using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class ArrangementRepository(ILogger logger, IDataService data) : IArrangementRepository
{
    public async Task<IEnumerable<ArrangementEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        var command = data.Connection.CreateCommand();
        if (ids is null)
        {
            command.CommandText = QueryConstants.GetAll.Replace("{table}", "`arrangement`");
        }
        else
        {
            command.CommandText = QueryConstants.GetById
                .Replace("{table}", "`arrangement`")
                .Replace("{values}", string.Join(",", ids));
        }
        var reader = await command.ExecuteReaderAsync();

        var arrangements = new List<ArrangementEntity>();
        while (reader.Read())
        {
            arrangements.Add(new ArrangementEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                BasePrice = reader.GetDecimal(2),
                Type = reader.GetString(3).MapToEnum<ArrangementType>(),
                CreatedAt = reader.GetDateTime(4),
                UpdatedAt = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5)
            });
        }

        return arrangements;
    }

    public async Task<IEnumerable<ArrangementEntity>> Create(IEnumerable<ArrangementEntity> arrangements)
    {
        if (data.Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        string[] columns = ["name", "base_price", "type"];
        string[] values = ["@name", "@basePrice", "@type"];

        var ids = new List<long>();
        foreach (var arrangement in arrangements.AsEnumerable())
        {
            long newId = 0;
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Insert
                    .Replace("{table}", "`arrangement`")
                    .Replace("{columns}", string.Join(",", columns))
                    .Replace("{values}", string.Join(",", values));

                command.Parameters.AddWithValue("@name", arrangement.Name);
                command.Parameters.AddWithValue("@basePrice", arrangement.BasePrice);
                command.Parameters.AddWithValue("@type", arrangement.Type.ToString());

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                newId = (long)result;

                logger.Debug("Arrangement with id {id} created.", newId);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error creating arrangement with id {Id}", arrangement.Id);
                continue;
            }
            ids.Add(newId);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<IEnumerable<ArrangementEntity>> Update(IEnumerable<ArrangementEntity> arrangements)
    {
        if (data.Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var ids = new List<long>();
        foreach (var arrangement in arrangements.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Update
                    .Replace("{table}", "`arrangement`")
                    .Replace("{columns}", string.Join(",", ["name = @name", "base_price = @basePrice", "type = @type"]));

                command.Parameters.AddWithValue("@id", arrangement.Id);
                command.Parameters.AddWithValue("@name", arrangement.Name);
                command.Parameters.AddWithValue("@basePrice", arrangement.BasePrice);
                command.Parameters.AddWithValue("@type", arrangement.Type.ToString());

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                var updated = (long)result;

                logger.Debug("Arrangement with id {Id} updated.", updated);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error updating arrangement with id {Id}", arrangement.Id);
                continue;
            }
            ids.Add(arrangement.Id);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<int> Delete(IEnumerable<ArrangementEntity> arrangements)
    {
        if (data.Connection == null)
        {
            logger.Error("Database connection is null.");
            return 0;
        }

        var deleted = 0;
        foreach (var arrangement in arrangements.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Delete
                    .Replace("{table}", "`arrangement`");

                command.Parameters.AddWithValue("@id", arrangement.Id);
                await command.ExecuteNonQueryAsync();

                logger.Debug("Arrangement with id {Id} deleted.", arrangement.Id);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error deleting arrangement with id {Id}", arrangement.Id);
                continue;
            }
            deleted++;
        }

        return deleted;
    }
}
