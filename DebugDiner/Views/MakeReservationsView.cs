using Terminal.Gui;

namespace DebugDiner;

public class MakeReservationsView : BaseView
{
    public MakeReservationsView() : base()
    {
        SetHeaderTitle("Make Reservations");
        SetContentTitle("Fill the form to make an reservation");

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var dateLabel = new Label
        {
            X = 0,
            Y = 0,
            Text = "Date:",
        };

        dateLabel.GetCurrentHeight(out var dateLabelHeight);

        var dateInput = new DateField
        {
            X = 0,
            Y = dateLabelHeight + dateLabel.Y,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Date = DateTime.Now
        };

        dateInput.GetCurrentHeight(out var dateInputHeight);

        var startTimeLabel = new Label
        {
            X = 0,
            Y = (dateInputHeight + dateInput.Y) + 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = "Start Time:",
        };

        var startTimeInput = new TimeField()
        {
            X = 0,
            Y = startTimeLabel.Y + 1,
            Width = Dim.Fill(),
            Time = DateTime.Now.TimeOfDay
        };

        var endTimeLabel = new Label
        {
            X = 0,
            Y = startTimeInput.Y + 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = "End Time:",
        };

        var endTimeInput = new TimeField()
        {
            X = 0,
            Y = endTimeLabel.Y + 1,
            Width = Dim.Fill(),
            Time = DateTime.Now.TimeOfDay
        };

        var guestsLabel = new Label
        {
            X = 0,
            Y = endTimeInput.Y + 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = "Amount of guests?:",
        };

        var guestInput = new TextField
        {
            X = 0,
            Y = guestsLabel.Y + 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var submitBtn = new Button
        {
            X = 0,
            Y = guestInput.Y + 1,
            Width = Dim.Fill(),
            AutoSize = true,
            Text = "Make Reservation",
        };

        container.Add(dateLabel, dateInput, startTimeLabel, startTimeInput, endTimeLabel, endTimeInput,
            guestsLabel, guestInput, submitBtn);

        SetContent(container);
    }
}
