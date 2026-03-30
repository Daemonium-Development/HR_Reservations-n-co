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
                TableId = reader.GetInt32(2),
                StartTime = DateTime.Parse(reader.GetString(3)),
                EndTime = DateTime.Parse(reader.GetString(4)),
                Guests = reader.GetInt32(5),
                Status = MapToEnum<ReservationStatus>(reader.GetString(6)),
                CreatedAt = DateTime.Parse(reader.GetString(7)),
                UpdatedAt = reader.IsDBNull(8) ? default : DateTime.Parse(reader.GetString(8))
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
                TableId = reader.GetInt32(2),
                StartTime = DateTime.Parse(reader.GetString(3)),
                EndTime = DateTime.Parse(reader.GetString(4)),
                Guests = reader.GetInt32(5),
                Status = MapToEnum<ReservationStatus>(reader.GetString(6)),
                CreatedAt = DateTime.Parse(reader.GetString(7)),
                UpdatedAt = reader.IsDBNull(8) ? default : DateTime.Parse(reader.GetString(8))
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
        
        var query = @"INSERT INTO reservation (user_id, table_id, start_time, end_time, guests, status)
                                VALUES (@userId, @tableId, @startTime, @endTime, @guests, @status);
                                SELECT last_insert_rowid();";

        List<ReservationEntity> createdReservations = [];

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
            
            var result = await command.ExecuteScalarAsync();
            var newId = (long)result;
            
            logger.Debug($"Reservation with id {newId} created.");
            
            var newCommand =  Connection.CreateCommand();
            newCommand.CommandText = "SELECT * FROM `reservation` WHERE id=@id";
            newCommand.Parameters.AddWithValue("@id", newId);

            var reader = await newCommand.ExecuteReaderAsync();
            while (reader.Read())
            {
                createdReservations.Add(new ReservationEntity
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TableId = reader.GetInt32(2),
                    StartTime = DateTime.Parse(reader.GetString(3)),
                    EndTime = DateTime.Parse(reader.GetString(4)),
                    Guests = reader.GetInt32(5),
                    Status = MapToEnum<ReservationStatus>(reader.GetString(6)),
                    CreatedAt = DateTime.Parse(reader.GetString(7)),
                    UpdatedAt = reader.IsDBNull(8) ? default : DateTime.Parse(reader.GetString(8))
                });
            }
        }

        return createdReservations;
    }

    public async Task<IEnumerable<ReservationEntity>> Update(IEnumerable<ReservationEntity> reservations)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return null;
        }
        
        var query = @"UPDATE reservation
                      SET user_id = @userId, table_id = @tableId, start_time = @startTime, end_time = @endTime,
                          guests = @guests, status = @status, updated_at = @updatedAt
                      WHERE id = @id;";

        List<ReservationEntity> updatedReservations = [];

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

            await command.ExecuteNonQueryAsync();

            logger.Debug($"Reservation with id {reservation.Id} updated.");

            var newCommand = Connection.CreateCommand();
            newCommand.CommandText = "SELECT * FROM `reservation` WHERE id=@id";
            newCommand.Parameters.AddWithValue("@id", reservation.Id);

            var reader = await newCommand.ExecuteReaderAsync();
            while (reader.Read())
            {
                updatedReservations.Add(new ReservationEntity
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TableId = reader.GetInt32(2),
                    StartTime = DateTime.Parse(reader.GetString(3)),
                    EndTime = DateTime.Parse(reader.GetString(4)),
                    Guests = reader.GetInt32(5),
                    Status = MapToEnum<ReservationStatus>(reader.GetString(6)),
                    CreatedAt = DateTime.Parse(reader.GetString(7)),
                    UpdatedAt = reader.IsDBNull(8) ? default : DateTime.Parse(reader.GetString(8))
                });
            }
        }

        return updatedReservations;
    }

    public async Task Delete(IEnumerable<ReservationEntity> reservations)
    {
        if (Connection == null)
        {
            logger.Error("Database connection is null.");
            return;
        }
        
        var query = "DELETE FROM reservation WHERE id = @id";

        foreach (var reservation in reservations.AsEnumerable())
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            
            command.Parameters.AddWithValue("@id", reservation.Id);
            await command.ExecuteNonQueryAsync();
            
            logger.Debug($"Reservation with id {reservation.Id} deleted.");
        }
    }
}