using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class UpdateReservationView : BaseView
{
    public UpdateReservationView(INavigationService nav, IReservationRepository repo) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Update Reservation");
        SetContentTitle("Update Reservation");

        var reservation = AppState.SelectedReservation;

        if (reservation is null)
        {
            nav.NavigateBack();
            return;
        }

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var guestsLabel = new Label("Guests:")
        {
            X = 2,
            Y = 2
        };

        var guestsInput = new TextField(reservation.Guests.ToString())
        {
            X = 2,
            Y = 3,
            Width = 20
        };

        var startLabel = new Label("Start time:")
        {
            X = 2,
            Y = 5
        };

        var startInput = new TextField(reservation.StartTime.ToString("yyyy-MM-dd HH:mm"))
        {
            X = 2,
            Y = 6,
            Width = 30
        };

        var endLabel = new Label("End time:")
        {
            X = 2,
            Y = 8
        };

        var endInput = new TextField(reservation.EndTime.ToString("yyyy-MM-dd HH:mm"))
        {
            X = 2,
            Y = 9,
            Width = 30
        };

        var saveBtn = new Button("Save")
        {
            X = 2,
            Y = 11
        };

        saveBtn.Clicked += () =>
        {
            if (int.TryParse(guestsInput.Text.ToString(), out var guests))
            {
                reservation.Guests = guests;
            }

            if (DateTime.TryParse(startInput.Text.ToString(), out var start))
            {
                reservation.StartTime = start;
            }

            if (DateTime.TryParse(endInput.Text.ToString(), out var end))
            {
                reservation.EndTime = end;
            }

            repo.Update([reservation]).GetAwaiter().GetResult();

            AppState.SelectedReservation = null;

            nav.NavigateBack();
        };

        container.Add(
            guestsLabel, guestsInput,
            startLabel, startInput,
            endLabel, endInput,
            saveBtn
        );

        SetContent(container);
    }
}