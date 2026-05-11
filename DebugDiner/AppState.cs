namespace DebugDiner;

public static class AppState
{
    public static UserEntity? CurrentUser { get; set; }
    public static UserEntity? SelectedUser { get; set; }
    public static ReservationEntity? SelectedReservation { get; set; }
}