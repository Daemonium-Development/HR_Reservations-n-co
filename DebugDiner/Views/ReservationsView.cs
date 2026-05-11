using DebugDiner.Domain.Abstractions;
using System.Collections.ObjectModel;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class ReservationsView : BaseView
{
    private readonly List<ReservationEntity> _reservations = new();

    public ReservationsView(INavigationService nav, IReservationRepository reservations) : base(nav)
    {
        var isAdmin = AppState.CurrentUser?.Role == Role.Admin;

        SetHeaderTitle(isAdmin
            ? "Reservations (Admin View)"
            : "My Reservations"
        );

        SetContentTitle(isAdmin
            ? "All System Reservations"
            : "My Personal Reservations"
        );

        LoadData(reservations, isAdmin);

        var grouped = _reservations
            .GroupBy(r => r.Status)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());

        var tabView = new TabView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        foreach (var (status, filtered) in grouped)
        {
            tabView.AddTab(CreateTab(status.ToString(), filtered, nav), false);
        }

        SetContent(tabView);
    }

    private void LoadData(IReservationRepository reservations, bool isAdmin)
    {
        var all = reservations.GetItemsAsync().GetAwaiter().GetResult();

        _reservations.Clear();

        _reservations.AddRange(
            isAdmin
                ? all
                : all.Where(r => r.UserId == AppState.CurrentUser!.Id)
        );
    }

    private static TabView.Tab CreateTab(
        string title,
        List<ReservationEntity> reservations,
        INavigationService nav)
    {
        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var header = new Label
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Text = $"{"ID",-5} {"User",-7} {"Table",-7} {"Start",-20} {"End",-20} {"Guests",-8} {"Status",-12}",
        };

        var items = new ObservableCollection<string>(
            reservations.Select(r =>
                $"{r.Id,-5} {r.UserId,-7} {r.TableId,-7} {r.StartTime,-20:dd-MM-yyyy HH:mm} {r.EndTime,-20:dd-MM-yyyy HH:mm} {r.Guests,-8} {r.Status,-12}"
            )
        );

        var listView = new ListView
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        listView.SetSource(items);

        listView.KeyPress += e =>
        {
            if (e.KeyEvent.Key != Key.Enter) return;

            var index = listView.SelectedItem;

            if (index < 0 || index >= reservations.Count) return;

            var reservation = reservations[index];

            AppState.SelectedReservation = reservation;

            var action = MessageBox.Query(
                "Reservation",
                "What do you want to do?",
                "Edit",
                "Delete",
                "Cancel"
            );

            if (action == 0)
            {
                nav.NavigateTo<UpdateReservationView>();
            }
            else if (action == 1)
            {
                nav.NavigateTo<DeleteReservationView>();
            }

            e.Handled = true;
        };

        container.Add(header, listView);

        return new TabView.Tab(title, container);
    }
}