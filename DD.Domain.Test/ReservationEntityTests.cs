using FluentAssertions;

namespace DD.Domain.Test;

public class ReservationEntityTests
{
    private static readonly DateTime Created = new(2024, 1, 1);
    private static readonly DateTime Updated = new(2024, 2, 1);
    private static readonly DateTime Start = new(2024, 3, 1, 18, 0, 0);
    private static readonly DateTime End = new(2024, 3, 1, 20, 0, 0);

    private static ReservationEntity Build() => new()
    {
        Id = 1,
        CreatedAt = Created,
        UpdatedAt = Updated,
        UserId = 5,
        TableId = 7,
        StartTime = Start,
        EndTime = End,
        Guests = 4,
        Status = ReservationStatus.Confirmed
    };

    [Fact]
    public void Equals_IdenticalReservations_AreEqual()
    {
        var a = Build();
        var b = Build();

        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentUserId_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.UserId = 99;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentTableId_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.TableId = 99;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentStartTime_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.StartTime = Start.AddHours(1);

        a.Equals(b).Should().BeFalse();
    }

    // Documents current behavior: Equals/GetHashCode deliberately ignore
    // EndTime, Guests and Status. Two reservations differing only in those
    // fields are still considered equal. This is the implemented behavior;
    // it is asserted, not "fixed" (see plan/02-layer-unit-tests.md).
    [Fact]
    public void Equals_DifferOnlyInStatus_AreStillEqual()
    {
        var a = Build();
        var b = Build();
        b.Status = ReservationStatus.Cancelled;

        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferOnlyInEndTimeAndGuests_AreStillEqual()
    {
        var a = Build();
        var b = Build();
        b.EndTime = End.AddHours(1);
        b.Guests = 99;

        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentType_IsFalse()
    {
        Build().Equals("not a reservation").Should().BeFalse();
    }
}
