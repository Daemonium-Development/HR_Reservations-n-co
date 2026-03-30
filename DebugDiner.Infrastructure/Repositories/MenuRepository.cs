using DebugDiner.Domain.Abstractions;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class MenuRepository(ILogger logger) : BaseRepository, IMenuRepository
{
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
                Description = reader.GetString(2),
                Price = reader.GetDecimal(3),
                DishCategory = Enum.Parse<DishCategory>(reader.GetString(4)),
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
                Description = reader.GetString(2),
                Price = reader.GetDecimal(3),
                DishCategory = Enum.Parse<DishCategory>(reader.GetString(4)),
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
                    VALUES (@name, @description, @price, @category, @allergen_info, @updatedAt)";

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
                    VALUES (@name, @description, @price, @category, @allergen_info, @updatedAt)";

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
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        logger.Information("Created {0} rows from database.", dishes.Count());
        return dishes;
    }

    public async Task Delete(IEnumerable<DishEntity> dishes)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return;
        }

        foreach (var dish in dishes.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = "DELETE FROM `dish` WHERE id=@id";
            command.Parameters.AddWithValue("@id", dish.Id);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        
        logger.Information("Deleted {0} rows from database.", dishes.Count());
    }
}