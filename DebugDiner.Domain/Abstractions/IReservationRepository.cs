namespace DebugDiner.Domain.Abstractions;

public interface IReservationRepository
{
    Task<IEnumerable<ReservationEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
    Task<IEnumerable<ReservationEntity>> Create(IEnumerable<ReservationEntity> reservations);
    Task<IEnumerable<ReservationEntity>> Update(IEnumerable<ReservationEntity> reservations);
    Task Delete(IEnumerable<ReservationEntity> reservations);
}