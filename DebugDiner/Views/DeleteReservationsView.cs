using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class DeleteReservationView : BaseView
{
    public DeleteReservationView(INavigationService nav, IReservationRepository reservationRepository) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Reservations");
        SetContentTitle("Delete Reservation");

        var reservation = AppState.SelectedReservation;

        if (reservation is null)
        {
            nav.NavigateTo<ReservationsView>();
            return;
        }

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var questionLabel = new Label
        {
            X = Pos.Center(),
            Y = 5,
            Text = "Are you sure you want to delete this reservation?",
        };

        var infoLabel = new Label
        {
            X = Pos.Center(),
            Y = 7,
            Text = $"Reservation ID: {reservation.Id}",
        };

        var yesBtn = new Button
        {
            X = 5,
            Y = 10,
            Text = "Yes, delete",
        };

        yesBtn.Clicked += () =>
        {
            try
            {
                reservationRepository.Delete([reservation]).GetAwaiter().GetResult();
                nav.NavigateTo<ReservationsView>();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to delete reservation: {ex.Message}", "OK");
            }
        };

        var noBtn = new Button
        {
            X = Pos.Right(yesBtn) + 5,
            Y = 10,
            Text = "No, go back",
        };

        noBtn.Clicked += () => nav.NavigateTo<ReservationsView>();

        container.Add(questionLabel, infoLabel, yesBtn, noBtn);

        SetContent(container);
    }
}