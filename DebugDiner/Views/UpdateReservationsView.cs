using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class UpdateReservationView : BaseView
{
    public UpdateReservationView(INavigationService nav, IReservationRepository reservationRepository) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Reservations");
        SetContentTitle("Update Reservation");

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

        var guestsLabel = new Label { X = 0, Y = 0, Text = "Guests:" };
        var guestsInput = new TextField { X = 0, Y = 1, Width = Dim.Fill(), Text = reservation.Guests.ToString() };

        var startLabel = new Label { X = 0, Y = 3, Text = "Start Time:" };
        var startInput = new TextField { X = 0, Y = 4, Width = Dim.Fill(), Text = reservation.StartTime.ToString() };

        var endLabel = new Label { X = 0, Y = 6, Text = "End Time:" };
        var endInput = new TextField { X = 0, Y = 7, Width = Dim.Fill(), Text = reservation.EndTime.ToString() };

        var submitBtn = new Button
        {
            X = 0,
            Y = 9,
            Text = "Update Reservation",
        };

        submitBtn.Clicked += () =>
        {
            try
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

                reservation.UpdatedAt = DateTime.UtcNow;

                reservationRepository.Update([reservation]).GetAwaiter().GetResult();

                nav.NavigateTo<ReservationsView>();
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
            submitBtn
        );

        SetContent(container);
    }
}