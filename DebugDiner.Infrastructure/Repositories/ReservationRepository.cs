using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class ReservationRepository(ILogger logger) : IReservationRepository
{
    public SqliteConnection? Connection { get; set; }
    
    public async Task<IEnumerable<ReservationEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
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
        
        var entityList = new List<ReservationEntity>();
        foreach (var id in ids)
        {
            var entity = await GetById(id);
            if (entity is null) continue;
            entityList.Add(entity);
        }
        return entityList;
    }
    
    private async Task<ReservationEntity?> GetById(int id)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }

        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM `reservation` WHERE id=@id"; 
        command.Parameters.AddWithValue("@id", id);

        var reader = await command.ExecuteReaderAsync();
        ReservationEntity? reservation = null;
        while (reader.Read())
        {
            reservation = new ReservationEntity
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                TableId = reader.GetInt32(2),
                StartTime = reader.GetDateTime(3),
                EndTime = reader.GetDateTime(4),
                Guests = reader.GetInt32(5),
                Status = reader.GetString(6).MapToEnum<ReservationStatus>(),
                CreatedAt = reader.GetDateTime(7),
                UpdatedAt = reader.GetDateTime(8)
            };
        }

        logger.Information("Reservation retrieved from database.");
        return reservation;
    }

    private async Task<IEnumerable<ReservationEntity>> GetAll()
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM `reservation`"; 

        var reader = await command.ExecuteReaderAsync();
        List<ReservationEntity> reservations = [];
        while (reader.Read())
        {
            reservations.Add(new ReservationEntity
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                TableId = reader.GetInt32(2),
                StartTime = reader.GetDateTime(3),
                EndTime = reader.GetDateTime(4),
                Guests = reader.GetInt32(5),
                Status = reader.GetString(6).MapToEnum<ReservationStatus>(),
                CreatedAt = reader.GetDateTime(7),
                UpdatedAt = reader.GetDateTime(8)
            });
        }

        logger.Information("Reservations retrieved from database.");
        return reservations;
    }

    public async Task<IEnumerable<ReservationEntity>> Create(IEnumerable<ReservationEntity> reservations)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        
        var query = @"INSERT INTO reservation (user_id, table_id, start_time, end_time, guests, status, created_at, updated_at) VALUES (@userId, @tableId, @startTime, @endTime, @guests, @status, @createdAt, @updatedAt) RETURNING id";

        var ids = new List<long>();
        foreach (var reservation in reservations.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            
            command.Parameters.AddWithValue("@userId", reservation.UserId);
            command.Parameters.AddWithValue("@tableId", reservation.TableId);
            command.Parameters.AddWithValue("@endTime", reservation.EndTime);
            command.Parameters.AddWithValue("@startTime", reservation.StartTime);
            command.Parameters.AddWithValue("@guests", reservation.Guests);
            command.Parameters.AddWithValue("@status", reservation.Status.ToString());
            command.Parameters.AddWithValue("@createdAt", DateTime.Now);
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
            
            var result = await command.ExecuteScalarAsync();
            var createdId = (long)result;
            
            logger.Debug("Reservation with id {newId} created.", createdId);
            if (createdId > 0) { ids.Add(createdId);}
        }
        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<IEnumerable<ReservationEntity>> Update(IEnumerable<ReservationEntity> reservations)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }
        
        var query = @"INSERT OR REPLACE INTO `reservation`
                        (id, user_id, table_id, start_time, end_time, guests, status, updated_at)
                        VALUES (@id, @userId, @tableId, @startTime, @endTime, @guests, @status, @updatedAt)
                        WHERE id = @id RETURNING id;";

        var ids = new List<long>();
        foreach (var reservation in reservations.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            
            command.Parameters.AddWithValue("@id", reservation.Id);
            command.Parameters.AddWithValue("@userId", reservation.UserId);
            command.Parameters.AddWithValue("@tableId", reservation.TableId);
            command.Parameters.AddWithValue("@startTime", reservation.StartTime);
            command.Parameters.AddWithValue("@endTime", reservation.EndTime);
            command.Parameters.AddWithValue("@guests", reservation.Guests);
            command.Parameters.AddWithValue("@status", reservation.Status.ToString());
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);

            var result = await command.ExecuteScalarAsync();
            var updatedId = (long)result;

            logger.Debug("Reservation with id {id} updated.", updatedId);
            if (updatedId > 0) { ids.Add(updatedId);}
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<int> Delete(IEnumerable<ReservationEntity> reservations)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return 0;
        }
        
        var query = "DELETE FROM reservation WHERE id = @id";

        var deleted = 0;
        foreach (var reservation in reservations.AsEnumerable())
        {
            try
            {
                var command = Connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@id", reservation.Id);
                await command.ExecuteNonQueryAsync();
                logger.Debug($"Reservation with id {reservation.Id} deleted.");
            }
            catch (SqliteException e)
            {
                logger.Error(e, $"Failed to delete reservation with id {reservation.Id}");
                continue;
            }
            deleted++;
        }
        
        return deleted;
    }
}