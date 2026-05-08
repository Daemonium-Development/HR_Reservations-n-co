using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class DeleteReservationView : BaseView
{
    public DeleteReservationView(INavigationService nav, IReservationRepository reservationRepository)
        : base(nav)
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

        var question = new Label
        {
            X = Pos.Center(),
            Y = 5,
            Text = "Are you sure you want to delete this reservation?"
        };

        var info = new Label
        {
            X = Pos.Center(),
            Y = 7,
            Text = $"Reservation #{reservation.Id}"
        };

        var yesBtn = new Button
        {
            X = 5,
            Y = 10,
            Text = "Yes"
        };

        var noBtn = new Button
        {
            X = Pos.Right(yesBtn) + 5,
            Y = 10,
            Text = "No"
        };

        yesBtn.Clicked += () =>
        {
            try
            {
                reservationRepository.Delete([reservation]).GetAwaiter().GetResult();

                AppState.SelectedReservation = null;

                nav.NavigateTo<ReservationsView>();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to delete: {ex.Message}", "OK");
            }
        };

        noBtn.Clicked += () =>
        {
            nav.NavigateTo<ReservationsView>();
        };

        yesBtn.CanFocus = true;
        noBtn.CanFocus = true;

        container.Add(question, info, yesBtn, noBtn);

        SetContent(container);

        container.SetFocus();
        yesBtn.SetFocus();
    }
}