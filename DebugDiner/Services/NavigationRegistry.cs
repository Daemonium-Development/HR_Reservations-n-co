using Terminal.Gui;

namespace DebugDiner.Services;

public static class NavigationRegistry
{
    private static Action<INavigationService> Logout => n => { AppState.CurrentUser = null; n.ClearHistory(); n.NavigateTo<WelcomeView>(); };

    private static readonly Dictionary<Type, Func<INavigationService, IEnumerable<NavigationItem>>> _registry = new()
    {
        // Entry / auth screens — no sidebar items
        [typeof(WelcomeView)]  = _ => [],
        [typeof(LoginView)]    = _ => [],
        [typeof(RegisterView)] = _ => [],

        // Transient CRUD screens — no sidebar items
        [typeof(CreateUserView)]         = _ => [],
        [typeof(UpdateUserView)]         = _ => [],
        [typeof(DeleteUserView)]         = _ => [],
        [typeof(CreateDishView)]         = _ => [],
        [typeof(UpdateDishView)]         = _ => [],
        [typeof(DeleteDishView)]         = _ => [],
        [typeof(CreateReservationsView)] = _ => [],
        [typeof(UpdateReservationView)]  = _ => [],
        [typeof(DeleteReservationView)]  = _ => [],

        // Primary navigation screens
        [typeof(HomeView)] = _ =>
        {
            var items = new List<NavigationItem>
            {
                new NavigationItem("Home",                 n => n.NavigateTo<HomeView>()),
                new NavigationItem("Make a Reservation",   n => n.NavigateTo<CreateReservationsView>()),
                new NavigationItem("View my Reservations", n => n.NavigateTo<ReservationsView>()),
                new NavigationItem("User Information",     n => n.NavigateTo<InformationView>()),
                new NavigationItem("Menu (Dishes)",        n => n.NavigateTo<DishView>()),
            };

            if (AppState.CurrentUser?.Role == Role.Admin)
            {
                items.AddRange([
                    new NavigationItem("Create Dish",        n => n.NavigateTo<CreateDishView>()),
                    new NavigationItem("Users",              n => n.NavigateTo<AdminUsersView>()),
                    new NavigationItem("Add User",           n => n.NavigateTo<CreateUserView>()),
                    new NavigationItem("Admin Reservations", n => n.NavigateTo<ReservationsView>()),
                ]);
            }

            items.Add(new NavigationItem("Logout", Logout));
            return items;
        },

        [typeof(ReservationsView)] = _ =>
        [
            new NavigationItem("Home",               n => n.NavigateTo<HomeView>()),
            new NavigationItem("Make a Reservation", n => n.NavigateTo<CreateReservationsView>()),
            new NavigationItem("Logout",             Logout),
        ],

        [typeof(InformationView)] = _ =>
        [
            new NavigationItem("Home",                 n => n.NavigateTo<HomeView>()),
            new NavigationItem("Make a Reservation",   n => n.NavigateTo<CreateReservationsView>()),
            new NavigationItem("View my Reservations", n => n.NavigateTo<ReservationsView>()),
            new NavigationItem("Logout",               Logout),
        ],

        [typeof(DishView)] = _ =>
        [
            new NavigationItem("Home",   n => n.NavigateTo<HomeView>()),
            new NavigationItem("Logout", Logout),
        ],

        [typeof(AdminUsersView)] = _ =>
        [
            new NavigationItem("Home",     n => n.NavigateTo<HomeView>()),
            new NavigationItem("Add User", n => n.NavigateTo<CreateUserView>()),
            new NavigationItem("Logout",   Logout),
        ],
    };

    public static IEnumerable<NavigationItem> GetItemsFor(Type viewType, INavigationService nav)
    {
        return _registry.TryGetValue(viewType, out var factory) ? factory(nav) : [];
    }
}
