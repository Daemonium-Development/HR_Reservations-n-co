using DebugDiner.Domain.Abstractions;

using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

/// <summary>
/// The User Repository as a child of <see cref="BaseRepository"/>.
/// We have the basic methods in here `GetById`, `GetAll`, and the `Create`, `Update`, `Delete` methods.
/// If need be, add more methods here that merge two tables together into a different object.
/// For now this should suffice.
/// Any of these methods should be placed in a `try / catch` block to handle any exceptions that may occur.
/// </summary>
/// <param name="logger"></param>
public class UserRepository(ILogger logger) : BaseRepository, IUserRepository
{
    public async Task<IEnumerable<UserEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
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

        var entityList = new List<UserEntity>();
        foreach (var id in ids)
        {
            var user = await GetById(id);
            if (user is null) continue;
            entityList.Add(user);
        }
        return entityList;
    }
    
    private async Task<UserEntity?> GetById(int id)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }

        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM `user` WHERE id=@id"; 
        command.Parameters.AddWithValue("@id", id);

        var reader = await command.ExecuteReaderAsync();
        UserEntity? user = null;
        while (reader.Read())
        {
            user = new UserEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                Role = MapToEnum<Role>(reader.GetString(4)),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };
        }

        logger.Information("User retrieved from database.");
        return user;
    }

    private async Task<IEnumerable<UserEntity>> GetAll()
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [  ];
        }

        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM `user`";

        var reader = await command.ExecuteReaderAsync();
        List<UserEntity> users = [];
        while (reader.Read())
        {
            users.Add(new UserEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                Role = MapToEnum<Role>(reader.GetString(4)),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            });
        }

        logger.Information("Retrieved {0} rows from database.", users.Count);
        return users;
    }

    public async Task<IEnumerable<UserEntity>> Create(IEnumerable<UserEntity> users)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        
        var query = "INSERT INTO `user` (name, email, password_hash, role, updated_at) VALUES (@name, @email, @passwordHash, @role, @updatedAt)";

        foreach (var user in users.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@role", user.Role.ToString());
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        logger.Information("Created {0} rows from database.", users.Count());
        return users;
    }

    public async Task<IEnumerable<UserEntity>> Update(IEnumerable<UserEntity> users)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        
        var query = "INSERT OR REPLACE INTO `user` (Id, name, email, password_hash, role, updated_at) VALUES (@Id, @name, @email, @passwordHash, @role, @updatedAt)";

        foreach (var user in users.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@role", user.Role.ToString());
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        logger.Information("Updated {0} rows from database.", users.Count());
        return users;
    }
    
    public async Task Delete(IEnumerable<UserEntity> users)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return;
        }

        foreach (var user in users.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = "DELETE FROM `user` WHERE id=@id";
            command.Parameters.AddWithValue("@id", user.Id);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        
        logger.Information("Deleted {0} rows from database.", users.Count());
    }
    
    // SUGGESTION:
    // add `public async Task<User?> BuildUser(int id)` method here.
    // This method will take in the id of the user and find the user, and their reservations, and return a User object.
}