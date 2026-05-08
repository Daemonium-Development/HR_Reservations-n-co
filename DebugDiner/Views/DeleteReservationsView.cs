using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class DeleteReservationView : BaseView
{
    public DeleteReservationView(INavigationService nav, IReservationRepository repo) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Delete Reservation");
        SetContentTitle("Delete Reservation");

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

        var label = new Label($"Delete reservation #{reservation.Id}?")
        {
            X = Pos.Center(),
            Y = 5
        };

        var yes = new Button("Yes")
        {
            X = Pos.Center() - 10,
            Y = 7
        };

        var no = new Button("No")
        {
            X = Pos.Center() + 2,
            Y = 7
        };

        yes.Clicked += () =>
        {
            repo.Delete([reservation]).GetAwaiter().GetResult();

            AppState.SelectedReservation = null;

            nav.NavigateBack();
        };

        no.Clicked += () => nav.NavigateBack();

        container.Add(label, yes, no);
        SetContent(container);
    }
}