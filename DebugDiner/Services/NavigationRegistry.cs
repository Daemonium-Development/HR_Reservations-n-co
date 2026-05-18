using Terminal.Gui;

namespace DebugDiner.Services;

public class NavigationRegistry
{
    private static readonly Dictionary<Type, Func<IEnumerable<string>>> _registry = new()
    {
        [typeof(CreateUserView)] = () => ["Home", "Users", "Logout"],

        [typeof(CreateDishView)] = () => ["Home", "Logout"],

        [typeof(HomeView)] = () =>
        {
            var items = new List<string>() { "Home", "Make a Reservation", "View my Reservations", "User Information"
                , "Menu (Dishes)" };
            if (AppState.CurrentUser?.Role == Role.Admin)
            {
                items.AddRange(["Create Dish", "Users", "Add User", "Admin Reservations"]);
            }

            items.Add("Logout");
            return items;
        },

        [typeof(InformationView)] = () => ["Home", "View Reservation(s)", "Logout"],

        [typeof(LoginView)]   = () => [],
        [typeof(WelcomeView)] = () => [],

        [typeof(DeleteUserView)]       = () => [],
        [typeof(CreateReservationsView)] = () => [],
        [typeof(AdminUsersView)]       = () => [],
        [typeof(ReservationsView)]     = () => [],
        [typeof(UpdateUserView)]       = () => []
        ,
    };

    public static IEnumerable<string> GetItemsFor(Type viewType) =>
        _registry.TryGetValue(viewType, out var factory)
            ? factory()
            : [];

    public static IEnumerable<string> GetItemsFor<TView>() where TView : View =>
        GetItemsFor(typeof(TView));
}
