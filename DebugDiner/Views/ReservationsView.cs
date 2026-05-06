using System.Data;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class ReservationsView : BaseView
{
    private readonly List<ReservationEntity> _reservations;

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

        var all = reservations.GetItemsAsync().GetAwaiter().GetResult();

        _reservations = (isAdmin
            ? all
            : all.Where(r => r.UserId == AppState.CurrentUser!.Id)).ToList();

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
            tabView.AddTab(CreateTab(status.ToString(), filtered), false);
        }

        SetContent(tabView);
    }

    private static TabView.Tab CreateTab(string title, List<ReservationEntity> reservations)
    {
        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var countLabel = new Label(5, 1, $"Total: {reservations.Count} reservations");

        var table = new DataTable();
        table.Columns.Add("ID");
        table.Columns.Add("User");
        table.Columns.Add("Table");
        table.Columns.Add("Start Time");
        table.Columns.Add("End Time");
        table.Columns.Add("Guests");
        table.Columns.Add("Status");

        var tableView = new TableView
        {
            X = 5,
            Y = 3,
            Width = Dim.Fill() - 10,
            Height = Dim.Fill() - 6,
            ColorScheme = LayoutView.DefaultColorScheme
        };

        foreach (var r in reservations)
        {
            table.Rows.Add(
                r.Id.ToString(),
                r.UserId.ToString(),
                r.TableId.ToString(),
                r.StartTime.ToString("dd-MM-yyyy HH:mm"),
                r.EndTime.ToString("dd-MM-yyyy HH:mm"),
                r.Guests.ToString(),
                r.Status.ToString()
            );
        }

        tableView.Table = table;

        container.Add(countLabel, tableView);

        return new TabView.Tab(title, container);
    }
}