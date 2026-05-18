using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class HomeView : BaseView
{
    public HomeView(INavigationService nav) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Home");
        SetContentTitle("Home");

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var welcomeLabel = new Label
        {
            X = 5,
            Y = 2,
            Text = $"Welcome, {AppState.CurrentUser?.Name ?? "Guest"}!",
        };

        container.Add(welcomeLabel);
        SetContent(container);
    }
}
