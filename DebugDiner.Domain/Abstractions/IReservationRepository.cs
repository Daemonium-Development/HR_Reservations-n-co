namespace DebugDiner.Domain.Abstractions;

public interface IReservationRepository
{
    Task<ReservationEntity?> GetById(int id);
    Task<IEnumerable<ReservationEntity>> GetAll();
    Task<IEnumerable<ReservationEntity>> Create(IEnumerable<ReservationEntity> reservations);
    Task<IEnumerable<ReservationEntity>> Update(IEnumerable<ReservationEntity> reservations);
    Task Delete(IEnumerable<ReservationEntity> reservations);
}