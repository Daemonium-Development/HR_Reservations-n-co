using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class MenuRepository(ILogger logger) : IMenuRepository
{
    public SqliteConnection? Connection { get; set; }
    
    public async Task<IEnumerable<DishEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
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

        var entityList = new List<DishEntity>();
        foreach (var id in ids)
        {
            var dish = await GetById(id);
            if (dish is null) continue;
            entityList.Add(dish);
        }
        return entityList;
    }
    
    private async Task<DishEntity?> GetById(int id)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }

        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM `dish` WHERE id=@id"; 
        command.Parameters.AddWithValue("@id", id);

        var reader = await command.ExecuteReaderAsync();
        DishEntity? dish = null;
        while (reader.Read())
        {
            dish = new DishEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Description = reader.GetString(3),
                DishCategory = reader.GetString(4).MapToEnum<DishCategory>(),
                AllergenInfo = reader.GetString(5),
                CreatedAt = reader.GetDateTime(6),
                UpdatedAt = reader.GetDateTime(7),
            };
        }

        logger.Information("Dish retrieved from database.");
        return dish;
    }

    private async Task<IEnumerable<DishEntity>> GetAll()
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM `dish`"; 
        
        var reader = await command.ExecuteReaderAsync();
        List<DishEntity> dishes = [];
        
        while (reader.Read())
        {
            dishes.Add(new DishEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Description = reader.GetString(3),
                DishCategory = reader.GetString(4).MapToEnum<DishCategory>(),
                AllergenInfo = reader.GetString(5),
                CreatedAt = reader.GetDateTime(6),
                UpdatedAt = reader.GetDateTime(7),
            });
        }

        logger.Information("Retrieved {0} rows from database.", dishes.Count);
        return dishes;
    }

    public async Task<IEnumerable<DishEntity>> Create(IEnumerable<DishEntity> dishes)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        
        var query = @"INSERT INTO `dish` (name, description, price, category, allergen_info, updated_at) 
                    VALUES (@name, @description, @price, @category, @allergen_info, @updatedAt) RETURNING id;";

        foreach (var dish in dishes.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@name", dish.Name);
            command.Parameters.AddWithValue("@description", dish.Description);
            command.Parameters.AddWithValue("@price", dish.Price);
            command.Parameters.AddWithValue("@category", dish.DishCategory.ToString());
            command.Parameters.AddWithValue("@allergen_info", dish.AllergenInfo);
            command.Parameters.AddWithValue("@createdAt", dish.CreatedAt);
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        logger.Information("Created {0} rows from database.", dishes.Count());
        return dishes;
    }

    public async Task<IEnumerable<DishEntity>> Update(IEnumerable<DishEntity> dishes)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        
        var query = @"INSERT OR REPLACE INTO `dish` (name, description, price, category, allergen_info, updated_at) 
                    VALUES (@name, @description, @price, @category, @allergen_info, @updatedAt) RETURNING id;";

        var ids = new List<long>();
        foreach (var dish in dishes.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@name", dish.Name);
            command.Parameters.AddWithValue("@description", dish.Description);
            command.Parameters.AddWithValue("@price", dish.Price);
            command.Parameters.AddWithValue("@category", dish.DishCategory.ToString());
            command.Parameters.AddWithValue("@allergen_info", dish.AllergenInfo);
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            var result = await command.ExecuteScalarAsync().ConfigureAwait(false);
            var updatedId = (long)result;
            
            logger.Debug("Dish with id {id} updated.", updatedId);
            if (updatedId > 0) { ids.Add(updatedId);}
        }

        logger.Information("Created {0} rows from database.", dishes.Count());
        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<int> Delete(IEnumerable<DishEntity> dishes)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return 0;
        }

        var deleted = 0;
        foreach (var dish in dishes.AsEnumerable())
        {
            try
            {
                var command = Connection.CreateCommand();
                command.CommandText = "DELETE FROM `dish` WHERE id=@id";
                command.Parameters.AddWithValue("@id", dish.Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error deleting dish with id {Id}", dish.Id);
                continue;
            }
            deleted++;
        }
        
        logger.Information("Deleted {0} rows from database.", dishes.Count());
        return deleted;
    }
}