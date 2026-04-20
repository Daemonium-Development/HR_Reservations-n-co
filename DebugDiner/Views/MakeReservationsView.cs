using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using System.Collections.ObjectModel;
using Terminal.Gui;

namespace DebugDiner;

public class MakeReservationsView : BaseView
{
    public MakeReservationsView(INavigationService nav, IReservationRepository reservationRepository, ITableRepository tableRepository) : base(nav)
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

        var tables = tableRepository.GetItemsAsync().GetAwaiter().GetResult().ToList();

        var tableLabel = new Label
        {
            X = 0,
            Y = guestInput.Y + 2,
            Text = "Table:",
        };

        var tableItems = tables
            .Select(t => $"[{t.Id}] {t.Type} (capacity {t.Capacity})")
            .ToList();

        var tableList = new ListView(tableItems)
        {
            X = 0,
            Y = tableLabel.Y + 1,
            Width = Dim.Fill(),
            Height = Math.Min(1, Math.Min(tables.Count, 5)),
        };
        tableList.SetSource(new ObservableCollection<string>(tableItems));

        var submitBtn = new Button
        {
            X = 0,
            Y = tableList.Y + 1,
            Width = Dim.Fill(),
            AutoSize = true,
            Text = "Make Reservation",
        };

        submitBtn.Clicked += () =>
        {
            if (AppState.CurrentUser is null) return;

            if (tableList.SelectedItem < 0 || tableList.SelectedItem >= tables.Count)
            {
                // no table selected — optionally show an error label
                return;
            }

            var selectedTable = tables[tableList.SelectedItem];

            if (!int.TryParse(guestInput.Text.ToString(), out var guests) || guests <= 0)
            {
                // invalid guest count — optionally show an error label
                return;
            }

            var startDateTime = dateInput.Date.Date + startTimeInput.Time;
            var endDateTime = dateInput.Date.Date + endTimeInput.Time;

            if (endDateTime <= startDateTime)
            {
                // invalid time range — optionally show an error label
                return;
            }

            var entity = new ReservationEntity
            {
                Id = 0,
                CreatedAt = DateTime.UtcNow,
                UserId = AppState.CurrentUser.Id,
                TableId = selectedTable.Id,
                StartTime = startDateTime,
                EndTime = endDateTime,
                Guests = guests,
                Status = ReservationStatus.Pending,
            };

            var result = reservationRepository.Create([entity]).GetAwaiter().GetResult();
            if (!result.Any())
            {
                // show error
                return;
            }

            MessageBox.Query("Success", "Reservation created.", "OK");
            nav.NavigateTo<HomeView>();
        };

        container.Add(dateLabel, dateInput, startTimeLabel, startTimeInput, endTimeLabel, endTimeInput,
            guestsLabel, guestInput, tableLabel, tableList, submitBtn);

        SetContent(container);
    }
}
