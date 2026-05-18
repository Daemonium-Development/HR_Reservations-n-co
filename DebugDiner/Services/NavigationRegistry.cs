using Terminal.Gui;

namespace DebugDiner.Services;

public static class NavigationRegistry
{
    private static Action<INavigationService> Logout =>
        n => { AppState.CurrentUser = null; n.ClearHistory(); n.NavigateTo<WelcomeView>(); };

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
                new("Home",                 n => n.NavigateTo<HomeView>()),
                new("Make a Reservation",   n => n.NavigateTo<CreateReservationsView>()),
                new("View my Reservations", n => n.NavigateTo<ReservationsView>()),
                new("User Information",     n => n.NavigateTo<InformationView>()),
                new("Menu (Dishes)",        n => n.NavigateTo<DishView>()),
            };

            if (AppState.CurrentUser?.Role == Role.Admin)
            {
                items.AddRange([
                    new("Create Dish",        n => n.NavigateTo<CreateDishView>()),
                    new("Users",              n => n.NavigateTo<AdminUsersView>()),
                    new("Add User",           n => n.NavigateTo<CreateUserView>()),
                    new("Admin Reservations", n => n.NavigateTo<ReservationsView>()),
                ]);
            }

            items.Add(new("Logout", Logout));
            return items;
        },

        [typeof(ReservationsView)] = _ =>
        [
            new("Home",               n => n.NavigateTo<HomeView>()),
            new("Make a Reservation", n => n.NavigateTo<CreateReservationsView>()),
            new("Logout",             Logout),
        ],

        [typeof(InformationView)] = _ =>
        [
            new("Home",                 n => n.NavigateTo<HomeView>()),
            new("Make a Reservation",   n => n.NavigateTo<CreateReservationsView>()),
            new("View my Reservations", n => n.NavigateTo<ReservationsView>()),
            new("Logout",               Logout),
        ],

        [typeof(DishView)] = _ =>
        [
            new("Home",   n => n.NavigateTo<HomeView>()),
            new("Logout", Logout),
        ],

        [typeof(AdminUsersView)] = _ =>
        [
            new("Home",     n => n.NavigateTo<HomeView>()),
            new("Add User", n => n.NavigateTo<CreateUserView>()),
            new("Logout",   Logout),
        ],
    };

    public static IEnumerable<NavigationItem> GetItemsFor(Type viewType, INavigationService nav) =>
        _registry.TryGetValue(viewType, out var factory) ? factory(nav) : [];
}