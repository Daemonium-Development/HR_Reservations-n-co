using System.Collections.ObjectModel;
using Terminal.Gui;

namespace DebugDiner;

public class ReservationsView : BaseView
{
    public ReservationsView(IEnumerable<ReservationEntity> reservations) : base()
    {
        SetHeaderTitle("Reservations");
        SetContentTitle("Booked Reservations");

        var grouped = reservations
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
            tabView.AddTab(CreateTab(status.ToString(), filtered), false);

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

        var columnHeader = new Label
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

        container.Add(columnHeader, listView);

        return new TabView.Tab(title, container);
    }
}
