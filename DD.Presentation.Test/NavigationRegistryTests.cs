using DebugDiner;
using DebugDiner.Services;

using FluentAssertions;

using Moq;

namespace DD.Presentation.Test;

// All tests mutate AppState.CurrentUser; kept in one class so xUnit serializes
// them, and reset in Dispose.
public class NavigationRegistryTests : IDisposable
{
    private readonly Mock<INavigationService> _nav = new();

    private IEnumerable<NavigationItem> ItemsFor(Type viewType) =>
        NavigationRegistry.GetItemsFor(viewType, _nav.Object);

    private static UserEntity User(Role role) => new()
    {
        Id = 1,
        CreatedAt = new DateTime(2024, 1, 1),
        Name = "Test",
        Email = "test@example.com",
        PasswordHash = "hash",
        Role = role
    };

    public static IEnumerable<object[]> EmptyItemViewTypes()
    {
        yield return [typeof(WelcomeView)];
        yield return [typeof(LoginView)];
        yield return [typeof(RegisterView)];
        yield return [typeof(CreateUserView)];
        yield return [typeof(UpdateUserView)];
        yield return [typeof(DeleteUserView)];
        yield return [typeof(CreateDishView)];
        yield return [typeof(UpdateDishView)];
        yield return [typeof(DeleteDishView)];
        yield return [typeof(CreateReservationsView)];
        yield return [typeof(UpdateReservationView)];
        yield return [typeof(DeleteReservationView)];
    }

    [Theory]
    [MemberData(nameof(EmptyItemViewTypes))]
    public void GetItemsFor_AuthAndTransientViews_AreEmpty(Type viewType)
    {
        ItemsFor(viewType).Should().BeEmpty();
    }

    [Fact]
    public void GetItemsFor_UnknownType_IsEmpty()
    {
        ItemsFor(typeof(string)).Should().BeEmpty();
    }

    [Fact]
    public void GetItemsFor_HomeView_NoUser_ReturnsSixNonAdminItems()
    {
        AppState.CurrentUser = null;

        ItemsFor(typeof(HomeView)).Select(i => i.Label).Should().Equal(
            "Home",
            "Make a Reservation",
            "View my Reservations",
            "User Information",
            "Menu (Dishes)",
            "Logout");
    }

    [Fact]
    public void GetItemsFor_HomeView_CustomerUser_ReturnsSixNonAdminItems()
    {
        AppState.CurrentUser = User(Role.Customer);

        ItemsFor(typeof(HomeView)).Select(i => i.Label).Should().Equal(
            "Home",
            "Make a Reservation",
            "View my Reservations",
            "User Information",
            "Menu (Dishes)",
            "Logout");
    }

    [Fact]
    public void GetItemsFor_HomeView_AdminUser_IncludesAdminItems()
    {
        AppState.CurrentUser = User(Role.Admin);

        ItemsFor(typeof(HomeView)).Select(i => i.Label).Should().Equal(
            "Home",
            "Make a Reservation",
            "View my Reservations",
            "User Information",
            "Menu (Dishes)",
            "Create Dish",
            "Users",
            "Add User",
            "Admin Reservations",
            "Logout");
    }

    [Fact]
    public void GetItemsFor_ReservationsView_ReturnsExpectedItems()
    {
        ItemsFor(typeof(ReservationsView)).Select(i => i.Label).Should().Equal(
            "Home",
            "Make a Reservation",
            "Logout");
    }

    [Fact]
    public void GetItemsFor_InformationView_ReturnsExpectedItems()
    {
        ItemsFor(typeof(InformationView)).Select(i => i.Label).Should().Equal(
            "Home",
            "Make a Reservation",
            "View my Reservations",
            "Logout");
    }

    [Fact]
    public void GetItemsFor_DishView_ReturnsExpectedItems()
    {
        ItemsFor(typeof(DishView)).Select(i => i.Label).Should().Equal(
            "Home",
            "Logout");
    }

    [Fact]
    public void GetItemsFor_AdminUsersView_ReturnsExpectedItems()
    {
        ItemsFor(typeof(AdminUsersView)).Select(i => i.Label).Should().Equal(
            "Home",
            "Add User",
            "Logout");
    }

    [Fact]
    public void LogoutAction_ClearsUserAndNavigatesToWelcome()
    {
        AppState.CurrentUser = User(Role.Customer);
        var logout = ItemsFor(typeof(HomeView)).Single(i => i.Label == "Logout");

        logout.Navigate(_nav.Object);

        AppState.CurrentUser.Should().BeNull();
        _nav.Verify(n => n.ClearHistory(), Times.Once);
        _nav.Verify(n => n.NavigateTo<WelcomeView>(), Times.Once);
    }

    public void Dispose()
    {
        AppState.CurrentUser = null;
    }
}
