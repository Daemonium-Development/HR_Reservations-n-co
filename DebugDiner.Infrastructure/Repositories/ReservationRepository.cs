using DebugDiner.Domain.Abstractions;

using Serilog;

namespace DebugDiner.Infrastructure.Repositories;

public class ReservationRepository(ILogger logger) : BaseRepository, IReservationRepository
{
    public async Task<ReservationEntity?> GetById(int id)
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
                TableId = reader.GetInt32(0),
                StartTime = DateTime.Parse(reader.GetString(1)),
                EndTime = DateTime.Parse(reader.GetString(2)),
                Guests = reader.GetInt32(3),
                Status = Enum.Parse<ReservationStatus>(reader.GetString(4)),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            };
        }

        logger.Information("Reservation retrieved from database.");
        return reservation;
    }

    public async Task<IEnumerable<ReservationEntity>> GetAll()
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
                TableId = reader.GetInt32(0),
                StartTime = DateTime.Parse(reader.GetString(1)),
                EndTime = DateTime.Parse(reader.GetString(2)),
                Guests = reader.GetInt32(3),
                Status = Enum.Parse<ReservationStatus>(reader.GetString(4)),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6)
            });
        }

        logger.Information("Reservations retrieved from database.");
        return reservations;
    }

    public async Task<IEnumerable<ReservationEntity>> Create(ReservationEntity reservation)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }
        
        var command = Connection.CreateCommand();
        command.CommandText = @"INSERT INTO reservation (user_id, table_id, start_time, end_time, guests, status)
                                VALUES (@userId, @tableId, @startTime, @endTime, @guests, @status);
                                SELECT last_insert_rowid();";
        
        command.Parameters.AddWithValue("@userId", reservation.UserId);
        command.Parameters.AddWithValue("@tableId", reservation.TableId);
        command.Parameters.AddWithValue("@endTime", reservation.EndTime);
        command.Parameters.AddWithValue("@startTime", reservation.StartTime);
        command.Parameters.AddWithValue("@guests", reservation.Guests);
        command.Parameters.AddWithValue("@status", reservation.Status.ToString());
        
        /*var result = await command.ExecuteScalarAsync();
        var newId = (long)result;
        
        logger.Debug($"Reservation with id {newId} created.");*/

        return null;
    }

    public Task<IEnumerable<ReservationEntity>> Update(ReservationEntity reservation)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReservationEntity>> Delete(int id)
    {
        throw new NotImplementedException();
    }
}