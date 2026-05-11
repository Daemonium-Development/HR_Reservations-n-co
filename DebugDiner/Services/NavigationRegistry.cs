using Terminal.Gui;

namespace DebugDiner.Services;

public class NavigationRegistry
{
    private static List<string> _baseItems = ["Home", "Logout"];
    private static readonly Dictionary<Type, Func<IEnumerable<string>>> _registry = new()
    {
        [typeof(AddUserView)] = () => BuildItems(["Users"]).ToArray(),

        [typeof(CreateDishView)] = () => BuildItems([]).ToArray(),

        [typeof(HomeView)] = () =>
        {
            var items = BuildItems(["Make a Reservation", "View my Reservations", "User Information"]).ToList();
            if (AppState.CurrentUser?.Role == Role.Admin)
            {
                items.Insert(items.Count - 2, "Create Dish");
                items.Insert(items.Count - 2, "Users");
                items.Insert(items.Count - 2, "Add User");
                items.Insert(items.Count - 2, "Admin Reservations");
            }

            return items;
        },

        [typeof(InformationView)] = () => BuildItems(["View Reservation(s)"]).ToArray(),

        [typeof(LoginView)]   = () => [],
        [typeof(WelcomeView)] = () => [],

        [typeof(DeleteUserView)]       = () => BuildItems([]),
        [typeof(MakeReservationsView)] = () => BuildItems([]),
        [typeof(AdminUsersView)]       = () => BuildItems([]),
        [typeof(ReservationsView)]     = () => BuildItems([]),
        [typeof(UpdateUserView)]       = () => BuildItems([])
        ,
    };

    public static IEnumerable<string> GetItemsFor(Type viewType) =>
        _registry.TryGetValue(viewType, out var factory)
            ? factory()
            : [];

    public static IEnumerable<string> GetItemsFor<TView>() where TView : View =>
        GetItemsFor(typeof(TView));

    private static IEnumerable<string> BuildItems(IEnumerable<string> items)
    {
        var pre = _baseItems;
        foreach (var item in items)
        {
            pre.Insert(1, item);
        }

        return pre;
    }
}
