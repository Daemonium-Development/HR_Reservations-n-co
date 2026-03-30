using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Utilities;
using Microsoft.Data.Sqlite;
using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class ReservationRepository(ILogger logger, IDataService data) : IReservationRepository
{
    public async Task<IEnumerable<ReservationEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }
        var command = data.Connection.CreateCommand();
        if (ids is null)
        {
            command.CommandText = QueryConstants.GetAll.Replace("{table}", "`reservation`");
        }
        else
        {
            command.CommandText = QueryConstants.GetById
                .Replace("{table}", "`reservation`")
                .Replace("{values}", string.Join(",", ids));
        }
        var reader = await command.ExecuteReaderAsync();

        var reservations = new List<ReservationEntity>();
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
                UpdatedAt = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8)
            });
        }

        return reservations;
    }

    public async Task<IEnumerable<ReservationEntity>> Create(IEnumerable<ReservationEntity> reservations)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        string[] columns = ["user_id", "table_id", "start_time", "end_time", "guests", "status"];
        string[] values = ["@userId", "@tableId", "@startTime", "@endTime", "@guests", "@status"];

        var ids = new List<long>();
        foreach (var reservation in reservations.AsEnumerable())
        {
            long newId = 0;
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Insert
                    .Replace("{table}", "`reservation`")
                    .Replace("{columns}", string.Join(",", columns))
                    .Replace("{values}", string.Join(",", values));

                command.Parameters.AddWithValue("@userId", reservation.UserId);
                command.Parameters.AddWithValue("@tableId", reservation.TableId);
                command.Parameters.AddWithValue("@startTime", reservation.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@endTime", reservation.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@guests", reservation.Guests);
                command.Parameters.AddWithValue("@status", reservation.Status.ToString());

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                newId = (long)result;

                logger.Debug("Reservation with id {Id} created.", newId);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error creating reservation with id {Id}", reservation.Id);
                continue;
            }
            ids.Add(newId);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<IEnumerable<ReservationEntity>> Update(IEnumerable<ReservationEntity> reservations)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return [];
        }

        var ids = new List<long>();
        foreach (var reservation in reservations.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Update
                    .Replace("{table}", "`reservation`")
                    .Replace("{columns}", string.Join(",", ["user_id = @userId", "table_id = @tableId", "start_time = @startTime", "end_time = @endTime", "guests = @guests", "status = @status"]));

                command.Parameters.AddWithValue("@id", reservation.Id);
                command.Parameters.AddWithValue("@userId", reservation.UserId);
                command.Parameters.AddWithValue("@tableId", reservation.TableId);
                command.Parameters.AddWithValue("@startTime", reservation.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@endTime", reservation.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@guests", reservation.Guests);
                command.Parameters.AddWithValue("@status", reservation.Status.ToString());

                var result = await command.ExecuteScalarAsync();

                if (result is null)
                    continue;
                var updated = (long)result;

                logger.Debug("Reservation with id {Id} updated.", updated);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error updating reservation with id {Id}", reservation.Id);
                continue;
            }
            ids.Add(reservation.Id);
        }

        return await GetItemsAsync(ids.Select(id => (int)id));
    }

    public async Task<int> Delete(IEnumerable<ReservationEntity> reservations)
    {
        if (data.Connection is null)
        {
            logger.Error("Database connection is null.");
            return 0;
        }

        var deleted = 0;
        foreach (var reservation in reservations.AsEnumerable())
        {
            try
            {
                var command = data.Connection.CreateCommand();
                command.CommandText = QueryConstants.Delete.Replace("{table}", "`reservation`");

                command.Parameters.AddWithValue("@id", reservation.Id);
                await command.ExecuteNonQueryAsync();

                logger.Debug("Reservation with id {Id} deleted.", reservation.Id);
            }
            catch (SqliteException e)
            {
                logger.Error(e, "Error deleting reservation with id {Id}", reservation.Id);
                continue;
            }
            deleted++;
        }

        return deleted;
    }
}
