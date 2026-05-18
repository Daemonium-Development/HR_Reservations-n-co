using System.Data;
using Terminal.Gui;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;

namespace DebugDiner;

public class ReservationsView : BaseView
{
    private readonly List<ReservationEntity> _reservations = [];

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
        try
        {
            var all = reservations.GetItemsAsync().GetAwaiter().GetResult();
            _reservations.Clear();
            _reservations.AddRange(
                isAdmin
                    ? all
                    : all.Where(r => r.UserId == AppState.CurrentUser!.Id)
            );
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to load reservations: {ex.Message}", "OK");
        }
    }

    private static TabView.Tab CreateTab(
        string title,
        List<ReservationEntity> reservations,
        INavigationService nav)
    {
        const int EditCol   = 6;
        const int DeleteCol = 7;

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var table = new DataTable();
        table.Columns.Add("ID");
        table.Columns.Add("User");
        table.Columns.Add("Table");
        table.Columns.Add("Start");
        table.Columns.Add("End");
        table.Columns.Add("Guests");
        table.Columns.Add("Edit");
        table.Columns.Add("Delete");

        foreach (var r in reservations)
        {
            table.Rows.Add(
                r.Id,
                r.UserId,
                r.TableId,
                r.StartTime.ToString("dd-MM-yyyy HH:mm"),
                r.EndTime.ToString("dd-MM-yyyy HH:mm"),
                r.Guests,
                "[Edit]",
                "[Delete]"
            );
        }

        var tableView = new TableView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = LayoutView.DefaultColorScheme,
            Table = table
        };

        tableView.CellActivated += (args) =>
        {
            if (args.Row >= reservations.Count)
            {
                return;
            }

            var reservation = reservations[args.Row];

            if (args.Col == EditCol)
            {
                AppState.SelectedReservation = reservation;
                nav.NavigateTo<UpdateReservationView>();
            }
            else if (args.Col == DeleteCol)
            {
                AppState.SelectedReservation = reservation;
                nav.NavigateTo<DeleteReservationView>();
            }
        };

        container.Add(tableView);

        return new TabView.Tab(title, container);
    }
}
