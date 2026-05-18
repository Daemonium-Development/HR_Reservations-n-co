using Terminal.Gui;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;

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

        var guestsLabel = new Label
        {
            X = 2,
            Y = 2,
            Text = "Guests:"
        };

        var guestsInput = new TextField
        {
            X = 2,
            Y = 3,
            Width = 20,
            Text = reservation.Guests.ToString()
        };

        var startLabel = new Label
        {
            X = 2,
            Y = 5,
            Text = "Start time:"
        };

        var startInput = new TextField
        {
            X = 2,
            Y = 6,
            Width = 30,
            Text = reservation.StartTime.ToString("dd-MM-yyyy HH:mm")
        };

        var endLabel = new Label
        {
            X = 2,
            Y = 8,
            Text = "End time:"
        };

        var endInput = new TextField
        {
            X = 2,
            Y = 9,
            Width = 30,
            Text = reservation.EndTime.ToString("dd-MM-yyyy HH:mm")
        };

        var saveBtn = new Button
        {
            X = 2,
            Y = 11,
            Text = "Save"
        };

        saveBtn.Clicked += () =>
        {
            var errors = new List<string>();

            if (!int.TryParse(guestsInput.Text.ToString(), out var guests))
            {
                errors.Add("Guests must be a valid number.");
            }

            if (!DateTime.TryParse(startInput.Text.ToString(), out var start))
            {
                errors.Add($"Start time must be a valid date ({"dd-MM-yyyy HH:mm"}).");
            }

            if (!DateTime.TryParse(endInput.Text.ToString(), out var end))
            {
                errors.Add($"End time must be a valid date ({"dd-MM-yyyy HH:mm"}).");
            }

            if (errors.Count > 0)
            {
                MessageBox.ErrorQuery("Validation Error", string.Join("\n", errors), "OK");
                return;
            }

            reservation.Guests    = guests;
            reservation.StartTime = start;
            reservation.EndTime   = end;

            try
            {
                repo.Update([reservation]).GetAwaiter().GetResult();
                AppState.SelectedReservation = null;
                nav.NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to update reservation: {ex.Message}", "OK");
            }
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
