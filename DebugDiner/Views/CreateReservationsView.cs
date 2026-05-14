using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class CreateReservationsView : BaseView
{
    public CreateReservationsView(
        INavigationService nav,
        IReservationRepository reservationRepository,
        ITableRepository tableRepository
    ) : base(nav)
    {
        SetHeaderTitle("Make Reservation");
        SetContentTitle("Select a table and fill in details");

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var dateLabel = new Label { X = 0, Y = 0, Text = "Date:" };

        var dateInput = new DateField
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Date = DateTime.Now
        };

        var startTimeLabel = new Label { X = 0, Y = 3, Text = "Start Time:" };

        var startTimeInput = new TimeField
        {
            X = 0,
            Y = 4,
            Width = Dim.Fill(),
            Time = DateTime.Now.TimeOfDay
        };

        var endTimeLabel = new Label { X = 0, Y = 6, Text = "End Time:" };

        var endTimeInput = new TimeField
        {
            X = 0,
            Y = 7,
            Width = Dim.Fill(),
            Time = DateTime.Now.TimeOfDay
        };

        var guestsLabel = new Label { X = 0, Y = 9, Text = "Guests:" };

        var guestInput = new TextField
        {
            X = 0,
            Y = 10,
            Width = Dim.Fill()
        };

        var tables = tableRepository.GetItemsAsync().GetAwaiter().GetResult().ToList();
        var reservations = reservationRepository.GetItemsAsync().GetAwaiter().GetResult().ToList();

        var takenTableIds = reservations
            .Where(r => r.Status == ReservationStatus.Pending ||
                        r.Status == ReservationStatus.Confirmed)
            .Select(r => r.TableId)
            .ToHashSet();

        var tableLabel = new Label { X = 0, Y = 12, Text = "Tables:" };

        var tableItems = tables.Select(t =>
        {
            var isTaken = takenTableIds.Contains(t.Id);

            return isTaken
                ? $"❌ [{t.Id}] {t.Type} (cap {t.Capacity}) - TAKEN"
                : $"✅ [{t.Id}] {t.Type} (cap {t.Capacity}) - AVAILABLE";
        }).ToList();

        var tableList = new ListView(tableItems)
        {
            X = 0,
            Y = 13,
            Width = Dim.Fill(),
            Height = 6
        };

        tableList.SelectedItemChanged += e =>
        {
            var selectedTable = tables[e.Item];

            if (takenTableIds.Contains(selectedTable.Id))
            {
                MessageBox.Query("Unavailable", "This table is already taken.", "OK");
            }
        };

        var submitBtn = new Button
        {
            X = 0,
            Y = 20,
            Text = "Make Reservation"
        };

        submitBtn.Clicked += () =>
        {
            if (AppState.CurrentUser is null)
                return;

            if (tableList.SelectedItem < 0)
                return;

            var selectedTable = tables[tableList.SelectedItem];

            if (takenTableIds.Contains(selectedTable.Id))
                return;

            if (!int.TryParse(guestInput.Text.ToString(), out var guests) || guests <= 0)
                return;

            var start = dateInput.Date.Date + startTimeInput.Time;
            var end = dateInput.Date.Date + endTimeInput.Time;

            if (end <= start)
                return;

            var entity = new ReservationEntity
            {
                Id = 0,
                CreatedAt = DateTime.UtcNow,
                UserId = AppState.CurrentUser.Id,
                TableId = selectedTable.Id,
                StartTime = start,
                EndTime = end,
                Guests = guests,
                Status = ReservationStatus.Pending
            };

            var result = reservationRepository.Create([entity]).GetAwaiter().GetResult();

            if (!result.Any())
                return;

            MessageBox.Query("Success", "Reservation created.", "OK");
            nav.NavigateTo<HomeView>();
        };

        container.Add(
            dateLabel, dateInput,
            startTimeLabel, startTimeInput,
            endTimeLabel, endTimeInput,
            guestsLabel, guestInput,
            tableLabel, tableList,
            submitBtn
        );

        SetContent(container);
    }
}
