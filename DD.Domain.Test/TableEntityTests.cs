using FluentAssertions;

namespace DD.Domain.Test;

public class TableEntityTests
{
    private static readonly DateTime Created = new(2024, 1, 1);
    private static readonly DateTime Updated = new(2024, 2, 1);

    private static TableEntity Build() => new()
    {
        Id = 1,
        CreatedAt = Created,
        UpdatedAt = Updated,
        Capacity = 4,
        Type = TableType.FourPerson
    };

    [Fact]
    public void Equals_IdenticalTables_AreEqual()
    {
        var a = Build();
        var b = Build();

        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentCapacity_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Capacity = 2;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentType_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Type = TableType.Bar;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentBaseField_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Id = 2;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentClrType_IsFalse()
    {
        Build().Equals("not a table").Should().BeFalse();
    }
}
