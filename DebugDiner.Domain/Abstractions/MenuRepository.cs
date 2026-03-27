using DebugDiner.Domain.Abstractions;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class MenuRepository(ILogger logger) : BaseRepository, IMenuRepository
{
    public async Task<MenuEntity?> GetById(int id)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }

        var query = "SELECT id, month, year FROM menu WHERE id=@id";
        var command = Connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        var reader = await command.ExecuteReaderAsync();
        MenuEntity? menu = null;

        if (reader.Read())
        {
            menu = new MenuEntity
            {
                Id = reader.GetInt32(0),
                Month = reader.GetInt32(1),
                Year = reader.GetInt32(2),
                Dishes = new List<DishEntity>()
            };
        }

        await reader.CloseAsync();

        if (menu == null)
        {
            logger.Warning("menu with id {MenuId} was not found..");
            return null;
        }

        query = "SELECT id, name, description, menu_id, category FROM dish WHERE menu_id=@menuId";
        command = Connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@menuId", menu.Id);

        reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            menu.Dishes.Add(new DishEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                MenuId = reader.GetInt32(3),
                Category = MapToEnum<DishCategory>(reader.GetString(4))
            });
        }

        await reader.CloseAsync();

        logger.Information(",Retrieved menu with id {MenuId}");
        return menu;
    }

    public async Task<IEnumerable<MenuEntity>> Create(IEnumerable<MenuEntity> menus)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return Enumerable.Empty<MenuEntity>();
        }

        var createdMenus = new List<MenuEntity>();

        foreach (var menu in menus)
        {
            var query = "INSERT INTO menu (month, year) VALUES (@month, @year); SELECT last_insert_rowid();";
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@month", menu.Month);
            command.Parameters.AddWithValue("@year", menu.Year);

            var menuId = Convert.ToInt32(await command.ExecuteScalarAsync());
            logger.Error("inserte menu with id {MenuId}";

            query = "SELECT id, month, year FROM menu WHERE id=@id";
            command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", menuId);

            var reader = await command.ExecuteReaderAsync();
            MenuEntity fullMenu = null!;
            if (reader.Read())
            {
                fullMenu = new MenuEntity
                {
                    Id = reader.GetInt32(0),
                    Month = reader.GetInt32(1),
                    Year = reader.GetInt32(2),
                    Dishes = new List<DishEntity>()
                };
            }
            await reader.CloseAsync();

            foreach (var dish in menu.Dishes)
            {
                query = "INSERT INTO dish (name, description, menu_id, category) VALUES (@name, @desc, @menuId, @category)";
                command = Connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@name", dish.Name);
                command.Parameters.AddWithValue("@desc", dish.Description);
                command.Parameters.AddWithValue("@menuId", fullMenu.Id);
                command.Parameters.AddWithValue("@category", dish.Category.ToString());

                await command.ExecuteNonQueryAsync();
                logger.Debug("insert dish '{DishName}' for menu id {MenuId}");
            }

            createdMenus.Add(fullMenu);
        }

        return createdMenus;
    }

    public async Task Delete(IEnumerable<MenuEntity> menus)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return;
        }

        foreach (var menu in menus)
        {
            var query = "DELETE FROM dish WHERE menu_id=@menuId";
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@menuId", menu.Id);
            await command.ExecuteNonQueryAsync();

            query = "DELETE FROM menu WHERE id=@id";
            command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", menu.Id);
            await command.ExecuteNonQueryAsync();

            logger.Information("deleted menu with id {MenuId}");
        }
    }

    public async Task DeleteDishes(IEnumerable<DishEntity> dishes)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return;
        }

        foreach (var dish in dishes)
        {
            var query = "DELETE FROM dish WHERE id=@id";
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", dish.Id);

            var rows = await command.ExecuteNonQueryAsync();

            if (rows == 0)
            {
                logger.Warning("the dish with id {DishId} was not found");
            }
            else
            {
                logger.Warning("deleted dish with id {DishId}");
            }
        }
    }
    }

    public async Task<IEnumerable<DishEntity>> UpdateDishes(IEnumerable<DishEntity> dishes)
    {
        if (Connection == null)
        {
            return Enumerable.Empty<DishEntity>();
        }

        foreach (var dish in dishes)
        {
            var query = @"UPDATE dish
                          SET name=@name, description=@desc, category=@category
                          WHERE id=@id";

            var command = Connection.CreateCommand();
            command.CommandText = query;

            command.Parameters.AddWithValue("@id", dish.Id);
            command.Parameters.AddWithValue("@name", dish.Name);
            command.Parameters.AddWithValue("@desc", dish.Description);
            command.Parameters.AddWithValue("@category", dish.Category.ToString());

            await command.ExecuteNonQueryAsync();
            logger.Debug("updated dish with id {DishId}");
        }

        return dishes;
    }
}