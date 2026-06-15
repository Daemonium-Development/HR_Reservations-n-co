using DebugDiner.Domain.Utilities;

using FluentAssertions;

namespace DD.Domain.Test;

public class RepositoryUtilitiesTests
{
    [Theory]
    [InlineData("Admin", Role.Admin)]
    [InlineData("Customer", Role.Customer)]
    [InlineData("Employee", Role.Employee)]
    [InlineData("Manager", Role.Manager)]
    public void MapToEnum_Role_ParsesValidValues(string value, Role expected)
    {
        value.MapToEnum<Role>().Should().Be(expected);
    }

    [Theory]
    [InlineData("Pending", ReservationStatus.Pending)]
    [InlineData("Confirmed", ReservationStatus.Confirmed)]
    [InlineData("Cancelled", ReservationStatus.Cancelled)]
    public void MapToEnum_ReservationStatus_ParsesValidValues(string value, ReservationStatus expected)
    {
        value.MapToEnum<ReservationStatus>().Should().Be(expected);
    }

    [Theory]
    [InlineData("TwoPerson", TableType.TwoPerson)]
    [InlineData("Bar", TableType.Bar)]
    public void MapToEnum_TableType_ParsesValidValues(string value, TableType expected)
    {
        value.MapToEnum<TableType>().Should().Be(expected);
    }

    [Theory]
    [InlineData("Meat", DishCategory.Meat)]
    [InlineData("Vegan", DishCategory.Vegan)]
    public void MapToEnum_DishCategory_ParsesValidValues(string value, DishCategory expected)
    {
        value.MapToEnum<DishCategory>().Should().Be(expected);
    }

    [Theory]
    [InlineData("ThreeCourse", ArrangementType.ThreeCourse)]
    [InlineData("Wine", ArrangementType.Wine)]
    public void MapToEnum_ArrangementType_ParsesValidValues(string value, ArrangementType expected)
    {
        value.MapToEnum<ArrangementType>().Should().Be(expected);
    }

    [Fact]
    public void MapToEnum_UnknownValue_Throws()
    {
        var act = () => "NotARole".MapToEnum<Role>();

        act.Should().Throw<ArgumentException>();
    }

    // Documents current behavior: Enum.Parse is called without ignoreCase,
    // so casing must match the enum member exactly.
    [Fact]
    public void MapToEnum_WrongCase_Throws()
    {
        var act = () => "admin".MapToEnum<Role>();

        act.Should().Throw<ArgumentException>();
    }
}
