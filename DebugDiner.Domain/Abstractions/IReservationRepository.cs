namespace DebugDiner.Domain.Abstractions;

public interface IReservationRepository
{
    Task<ReservationEntity?> GetById(int id);
    Task<IEnumerable<ReservationEntity>> GetAll();
    // TODO: Kijken of de create een IEnumerable<ReservationEntity> returnen moet of een INT.
    Task<IEnumerable<ReservationEntity>> Create(ReservationEntity reservation);
    Task<IEnumerable<ReservationEntity>> Update(ReservationEntity reservation);
    Task<IEnumerable<ReservationEntity>> Delete(int id);
}