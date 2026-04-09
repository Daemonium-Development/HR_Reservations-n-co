using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class HomeView : BaseView
{
    public HomeView(INavigationService nav) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Home");
        SetContentTitle("Home");
        SetNavigationItems(
            "Home",
            "Make a reservation",
            "View reservations",
            "User information"
        );
        if (AppState.CurrentUser?.Role == Role.Admin)
        {
            SetNavigationItems("Create Dish", "Users", "Add User", "Reservations");
        }
        SetNavigationItems("Logout");

        NavigationMenu.OpenSelectedItem += (ListViewItemEventArgs e) =>
        {
            switch ((e.Item, AppState.CurrentUser?.Role))
            {
                case (0, _):
                    nav.NavigateTo<HomeView>();
                    break;
                case (1, _):
                    nav.NavigateTo<MakeReservationsView>();
                    break;
                case (2, _):
                    nav.NavigateTo<ReservationsView>();
                    break;
                case (3, _):
                    nav.NavigateTo<InformationView>();
                    break;
                case (4, Role.Admin):
                    nav.NavigateTo<CreateDishView>();
                    break;
                case (5, Role.Admin):
                    nav.NavigateTo<AdminUsersView>();
                    break;
                case (6, Role.Admin):
                    nav.NavigateTo<AddUserView>();
                    break;
                case (7, Role.Admin):
                    nav.NavigateTo<ReservationsView>();
                    break;
                case (8, Role.Admin):
                    AppState.CurrentUser = null;
                    nav.NavigateTo<WelcomeView>();
                    break;
                case (4, _):
                    AppState.CurrentUser = null;
                    nav.NavigateTo<WelcomeView>();
                    break;
            }
        };

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
