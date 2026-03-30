using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class ArrangementRepository(ILogger logger) : IArrangementRepository
{
    public SqliteConnection? Connection { get; set; }
    
    public async Task<IEnumerable<ArrangementEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
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
        
        var entityList = new List<ArrangementEntity>();
        foreach (var id in ids)
        {
            var entity = await GetById(id);
            if (entity is null) continue;
            entityList.Add(entity);
        }
        return entityList;
    }
    
    private async Task<ArrangementEntity?> GetById(int id)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }

        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM `arrangement` WHERE id=@id"; 
        command.Parameters.AddWithValue("@id", id);

        var reader = await command.ExecuteReaderAsync();
        ArrangementEntity? arrangement = null;
        while (reader.Read())
        {
            arrangement = new ArrangementEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                BasePrice = reader.GetInt64(2),
                Type = reader.GetString(3).MapToEnum<ArrangementType>(),
                CreatedAt = DateTime.Parse(reader.GetString(4)),
                UpdatedAt = reader.IsDBNull(5) ? default : DateTime.Parse(reader.GetString(5))
            };
        }

        logger.Information("Arrangement retrieved from database.");
        return arrangement;
    }

    private async Task<IEnumerable<ArrangementEntity>> GetAll()
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM `arrangement`"; 

        var reader = await command.ExecuteReaderAsync();
        List<ArrangementEntity> arrangements = [];
        while (reader.Read())
        {
            arrangements.Add(new ArrangementEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                BasePrice = reader.GetInt64(2),
                Type = reader.GetString(3).MapToEnum<ArrangementType>(),
                CreatedAt = DateTime.Parse(reader.GetString(4)),
                UpdatedAt = reader.IsDBNull(5) ? default : DateTime.Parse(reader.GetString(5))
            });
        }

        logger.Information("Arrangements retrieved from database.");
        return arrangements;
    }

    public async Task<IEnumerable<ArrangementEntity>> Create(IEnumerable<ArrangementEntity> arrangements)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        
        var query = @"INSERT INTO arrangement (name, base_price, type, updated_at)
                                VALUES (@name, @basePrice, @type, @updatedAt);
                                RETURNING id;";

        List<ArrangementEntity> createdArrangements = [];

        foreach (var arrangement in arrangements.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            
            command.Parameters.AddWithValue("@name", arrangement.Name);
            command.Parameters.AddWithValue("@basePrice", arrangement.BasePrice);
            command.Parameters.AddWithValue("@type", arrangement.Type.ToString());
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            
            var result = await command.ExecuteScalarAsync();
            
            if (result is null) continue;
            var newId = (long)result;
            
            logger.Debug($"Arrangement with id {newId} created.");
            
            var newCommand =  Connection.CreateCommand();
            newCommand.CommandText = "SELECT * FROM `arrangement` WHERE id=@id";
            newCommand.Parameters.AddWithValue("@id", newId);

            var reader = await newCommand.ExecuteReaderAsync();
            while (reader.Read())
            {
                createdArrangements.Add(new ArrangementEntity
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    BasePrice = reader.GetInt64(2),
                    Type = reader.GetString(3).MapToEnum<ArrangementType>(),
                    CreatedAt = DateTime.Parse(reader.GetString(4)),
                    UpdatedAt = reader.IsDBNull(5) ? default : DateTime.Parse(reader.GetString(5))
                });
            }
        }

        return createdArrangements;
    }

    public async Task<IEnumerable<ArrangementEntity>> Update(IEnumerable<ArrangementEntity> arrangements)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }
        
        var query = @"UPDATE arrangement
                      SET name = @name, base_price = @basePrice, type = @type, updated_at = @updatedAt
                      WHERE id = @id;";

        List<ArrangementEntity> updatedArrangements = [];

        foreach (var arrangement in arrangements.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            
            command.Parameters.AddWithValue("@id", arrangement.Id);
            command.Parameters.AddWithValue("@name", arrangement.Name);
            command.Parameters.AddWithValue("@basePrice", arrangement.BasePrice);
            command.Parameters.AddWithValue("@type", arrangement.Type.ToString());
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);

            await command.ExecuteNonQueryAsync();

            logger.Debug($"Arrangement with id {arrangement.Id} updated.");

            var newCommand = Connection.CreateCommand();
            newCommand.CommandText = "SELECT * FROM `arrangement` WHERE id=@id";
            newCommand.Parameters.AddWithValue("@id", arrangement.Id);

            var reader = await newCommand.ExecuteReaderAsync();
            while (reader.Read())
            {
                updatedArrangements.Add(new ArrangementEntity
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    BasePrice = reader.GetInt64(2),
                    Type = reader.GetString(3).MapToEnum<ArrangementType>(),
                    CreatedAt = DateTime.Parse(reader.GetString(4)),
                    UpdatedAt = reader.IsDBNull(5) ? default : DateTime.Parse(reader.GetString(5))
                });
            }
        }

        return updatedArrangements;
    }

    public async Task Delete(IEnumerable<ArrangementEntity> arrangements)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return;
        }
        
        var query = "DELETE FROM arrangement WHERE id = @id";

        foreach (var arrangement in arrangements.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            
            command.Parameters.AddWithValue("@id", arrangement.Id);
            await command.ExecuteNonQueryAsync();
            
            logger.Debug($"Arrangement with id {arrangement.Id} deleted.");
        }
    }
}