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

        var dateLabel = new Label
        {
            X = 0,
            Y = 0,
            Text = "Date:"
        };

        var dateInput = new DateField
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Date = DateTime.Now
        };

        var startTimeLabel = new Label
        {
            X = 0,
            Y = 3,
            Text = "Reservation Time:"
        };

        var slotTimes = GenerateTimeSlots();

        var timeSlotItems = slotTimes
            .Select(t => t.ToString(@"hh\:mm"))
            .ToList();

        var timeSlotList = new ListView(timeSlotItems)
        {
            X = 0,
            Y = 4,
            Width = Dim.Fill(),
            Height = 5
        };

        timeSlotList.SelectedItem = 0;

        var guestsLabel = new Label
        {
            X = 0,
            Y = 10,
            Text = "Guests:"
        };

        var guestInput = new TextField
        {
            X = 0,
            Y = 11,
            Width = Dim.Fill()
        };

        var tables = tableRepository
            .GetItemsAsync()
            .GetAwaiter()
            .GetResult()
            .ToList();

        var reservations = reservationRepository
            .GetItemsAsync()
            .GetAwaiter()
            .GetResult()
            .ToList();

        var tableLabel = new Label
        {
            X = 0,
            Y = 13,
            Text = "Tables:"
        };

        var tableList = new ListView(new List<string>())
        {
            X = 0,
            Y = 14,
            Width = Dim.Fill(),
            Height = 6
        };

        void RefreshTableAvailability()
        {
            if (timeSlotList.SelectedItem < 0)
            {
                return;
            }

            var selectedTime = slotTimes[timeSlotList.SelectedItem];

            var start = dateInput.Date.Date + selectedTime;
            var end = start.AddHours(2);

            var unavailableTableIds = reservations
                .Where(r =>
                    (r.Status == ReservationStatus.Pending ||
                     r.Status == ReservationStatus.Confirmed)
                    &&
                    start < r.EndTime &&
                    end > r.StartTime
                )
                .Select(r => r.TableId)
                .ToHashSet();

            var items = tables.Select(t =>
            {
                var isTaken = unavailableTableIds.Contains(t.Id);

                return isTaken
                    ? $"❌ [{t.Id}] {t.Type} (cap {t.Capacity}) - TAKEN"
                    : $"✅ [{t.Id}] {t.Type} (cap {t.Capacity}) - AVAILABLE";
            }).ToList();

            tableList.SetSource(items);
        }

        timeSlotList.SelectedItemChanged += (_) => RefreshTableAvailability();

        dateInput.DateChanged += (_) => RefreshTableAvailability();

        var submitBtn = new Button
        {
            X = 0,
            Y = 21,
            Text = "Make Reservation"
        };

        submitBtn.Clicked += () =>
        {
            if (AppState.CurrentUser is null)
            {
                return;
            }

            if (tableList.SelectedItem < 0 ||
                timeSlotList.SelectedItem < 0)
            {
                return;
            }

            if (!int.TryParse(guestInput.Text.ToString(), out var guests) ||
                guests <= 0)
            {
                return;
            }

            var selectedTable = tables[tableList.SelectedItem];

            var selectedTime = slotTimes[timeSlotList.SelectedItem];

            var start = dateInput.Date.Date + selectedTime;

            var end = start.AddHours(2);

            var hasConflict = reservations.Any(r =>
                r.TableId == selectedTable.Id
                &&
                (r.Status == ReservationStatus.Pending ||
                 r.Status == ReservationStatus.Confirmed)
                &&
                start < r.EndTime
                &&
                end > r.StartTime
            );

            if (hasConflict)
            {
                MessageBox.ErrorQuery(
                    "Unavailable",
                    "This table is already reserved for this time slot.",
                    "OK"
                );

                return;
            }

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

            var result = reservationRepository
                .Create([entity])
                .GetAwaiter()
                .GetResult();

            if (!result.Any())
            {
                return;
            }

            MessageBox.Query(
                "Success",
                $"Reservation created.\n\nEnds at: {end:HH:mm}",
                "OK"
            );

            nav.NavigateBack();
        };

        container.Add(
            dateLabel,
            dateInput,

            startTimeLabel,
            timeSlotList,

            guestsLabel,
            guestInput,

            tableLabel,
            tableList,

            submitBtn
        );

        SetContent(container);

        RefreshTableAvailability();
    }

    private static List<TimeSpan> GenerateTimeSlots()
    {
        var slots = new List<TimeSpan>();

        var start = new TimeSpan(17, 0, 0);
        var end = new TimeSpan(22, 0, 0);

        while (start <= end)
        {
            slots.Add(start);
            start += TimeSpan.FromMinutes(15);
        }

        return slots;
    }
}