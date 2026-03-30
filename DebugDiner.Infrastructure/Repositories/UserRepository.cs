using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class UserRepository(ILogger logger, IDataService data) : IUserRepository
{
    public async Task<IEnumerable<UserEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        var command = data.Connection.CreateCommand();
        if (ids is null)
        {
            command.CommandText = QueryConstants.GetAll.Replace("{table}", "`user`");
        }
        else
        {
            command.CommandText = QueryConstants.GetById
                .Replace("{table}", "`user`")
                .Replace("{values}", string.Join(",", ids));
        }
        var reader = await command.ExecuteReaderAsync();

        var users = new List<UserEntity>();
        while (reader.Read())
        {
            users.Add(new UserEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                Role = reader.GetString(4).MapToEnum<Role>(),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6)
            });
        }

        return users;
    }

    public async Task<IEnumerable<UserEntity>> Create(IEnumerable<UserEntity> users)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        string[] columns = ["name", "email", "password_hash", "role"];
        string[] values = ["@name", "@email", "@passwordHash", "@role"];

        var ids = new List<long>();
        foreach (var user in users.AsEnumerable())
        {
            long newId = 0;
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Insert
                    .Replace("{table}", "`user`")
                    .Replace("{columns}", string.Join(",", columns))
                    .Replace("{values}", string.Join(",", values));

                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@role", user.Role.ToString());

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                newId = (long)result;

                logger.Debug("User with id {Id} created.", newId);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error creating user with id {Id}", user.Id);
                continue;
            }
            ids.Add(newId);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<IEnumerable<UserEntity>> Update(IEnumerable<UserEntity> users)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var ids = new List<long>();
        foreach (var user in users.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Update
                    .Replace("{table}", "`user`")
                    .Replace("{columns}", string.Join(",", ["name = @name", "email = @email", "password_hash = @passwordHash", "role = @role"]));

                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@role", user.Role.ToString());

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                var updated = (long)result;

                logger.Debug("User with id {Id} updated.", updated);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error updating user with id {Id}", user.Id);
                continue;
            }
            ids.Add(user.Id);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<int> Delete(IEnumerable<UserEntity> users)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return 0;
        }

        var deleted = 0;
        foreach (var user in users.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Delete.Replace("{table}", "`user`");

                command.Parameters.AddWithValue("@id", user.Id);
                await command.ExecuteNonQueryAsync();

                logger.Debug("User with id {Id} deleted.", user.Id);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error deleting user with id {Id}", user.Id);
                continue;
            }
            deleted++;
        }

        return deleted;
    }
}
