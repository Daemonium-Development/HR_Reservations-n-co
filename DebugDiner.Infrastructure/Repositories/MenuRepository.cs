using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class MenuRepository(ILogger logger, IDataService data) : IMenuRepository
{
    public async Task<IEnumerable<DishEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        var command = data.Connection.CreateCommand();
        if (ids is null)
        {
            command.CommandText = QueryConstants.GetAll.Replace("{table}", "`dish`");
        }
        else
        {
            command.CommandText = QueryConstants.GetById
                .Replace("{table}", "`dish`")
                .Replace("{values}", string.Join(",", ids));
        }
        var reader = await command.ExecuteReaderAsync();

        var dishes = new List<DishEntity>();
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
                UpdatedAt = reader.IsDBNull(7) ? DateTime.Now : reader.GetDateTime(7)
            });
        }

        return dishes;
    }

    public async Task<IEnumerable<DishEntity>> Create(IEnumerable<DishEntity> dishes)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        string[] columns = ["name", "price", "description", "category", "allergen_info"];
        string[] values = ["@name", "@price", "@description", "@category", "@allergenInfo"];

        var ids = new List<long>();
        foreach (var dish in dishes.AsEnumerable())
        {
            long newId = 0;
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Insert
                    .Replace("{table}", "`dish`")
                    .Replace("{columns}", string.Join(",", columns))
                    .Replace("{values}", string.Join(",", values));

                command.Parameters.AddWithValue("@name", dish.Name);
                command.Parameters.AddWithValue("@price", dish.Price);
                command.Parameters.AddWithValue("@description", dish.Description);
                command.Parameters.AddWithValue("@category", dish.DishCategory.ToString());
                command.Parameters.AddWithValue("@allergenInfo", dish.AllergenInfo);

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                newId = (long)result;

                logger.Debug("Dish with id {Id} created.", newId);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error creating dish with id {Id}", dish.Id);
                continue;
            }
            ids.Add(newId);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<IEnumerable<DishEntity>> Update(IEnumerable<DishEntity> dishes)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var ids = new List<long>();
        foreach (var dish in dishes.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Update
                    .Replace("{table}", "`dish`")
                    .Replace("{columns}", string.Join(",", ["name = @name", "price = @price", "description = @description", "category = @category", "allergen_info = @allergenInfo"]));

                command.Parameters.AddWithValue("@id", dish.Id);
                command.Parameters.AddWithValue("@name", dish.Name);
                command.Parameters.AddWithValue("@price", dish.Price);
                command.Parameters.AddWithValue("@description", dish.Description);
                command.Parameters.AddWithValue("@category", dish.DishCategory.ToString());
                command.Parameters.AddWithValue("@allergenInfo", dish.AllergenInfo);

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                var updated = (long)result;

                logger.Debug("Dish with id {Id} updated.", updated);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error updating dish with id {Id}", dish.Id);
                continue;
            }
            ids.Add(dish.Id);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<int> Delete(IEnumerable<DishEntity> dishes)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return 0;
        }

        var deleted = 0;
        foreach (var dish in dishes.AsEnumerable())
        {
            try
            {
                var deleteArrangementDish = data.Connection.CreateCommand();
                deleteArrangementDish.CommandText = "DELETE FROM `arrangement_dish` WHERE `dish_id` = @id";
                deleteArrangementDish.Parameters.AddWithValue("@id", dish.Id);
                await deleteArrangementDish.ExecuteNonQueryAsync();

                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Delete.Replace("{table}", "`dish`");

                command.Parameters.AddWithValue("@id", dish.Id);
                await command.ExecuteNonQueryAsync();

                logger.Debug("Dish with id {Id} deleted. {Command}", dish.Id, command.CommandText.ToString());
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error deleting dish with id {Id}", dish.Id);
                continue;
            }
            deleted++;
        }

        return deleted;
    }
}
