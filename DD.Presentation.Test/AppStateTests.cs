using DebugDiner;

using FluentAssertions;

namespace DD.Presentation.Test;

public class AppStateTests : IDisposable
{
    private static readonly DateTime Created = new(2024, 1, 1);

    private static UserEntity BuildUser() => new()
    {
        Id = 1,
        CreatedAt = Created,
        Name = "Alice",
        Email = "alice@example.com",
        PasswordHash = "hash",
        Role = Role.Customer
    };

    private static DishEntity BuildDish() => new()
    {
        Id = 1,
        CreatedAt = Created,
        Name = "Steak",
        Description = "Grilled",
        Price = 10m,
        DishCategory = DishCategory.Meat
    };

    private static ReservationEntity BuildReservation() => new()
    {
        Id = 1,
        CreatedAt = Created,
        UserId = 1,
        TableId = 1,
        StartTime = Created,
        EndTime = Created,
        Guests = 2,
        Status = ReservationStatus.Pending
    };

    [Fact]
    public void CurrentUser_RoundTrips()
    {
        var user = BuildUser();

        AppState.CurrentUser = user;
        AppState.CurrentUser.Should().BeSameAs(user);

        AppState.CurrentUser = null;
        AppState.CurrentUser.Should().BeNull();
    }

    [Fact]
    public void SelectedUser_RoundTrips()
    {
        var user = BuildUser();

        AppState.SelectedUser = user;
        AppState.SelectedUser.Should().BeSameAs(user);

        AppState.SelectedUser = null;
        AppState.SelectedUser.Should().BeNull();
    }

    [Fact]
    public void SelectedDish_RoundTrips()
    {
        var dish = BuildDish();

        AppState.SelectedDish = dish;
        AppState.SelectedDish.Should().BeSameAs(dish);

        AppState.SelectedDish = null;
        AppState.SelectedDish.Should().BeNull();
    }

    [Fact]
    public void SelectedReservation_RoundTrips()
    {
        var reservation = BuildReservation();

        AppState.SelectedReservation = reservation;
        AppState.SelectedReservation.Should().BeSameAs(reservation);

        AppState.SelectedReservation = null;
        AppState.SelectedReservation.Should().BeNull();
    }

    // AppState is static mutable global state; reset it after every test.
    public void Dispose()
    {
        AppState.CurrentUser = null;
        AppState.SelectedUser = null;
        AppState.SelectedDish = null;
        AppState.SelectedReservation = null;
    }
}
