using FluentAssertions;

namespace DD.Domain.Test;

public class ArrangementEntityTests
{
    private static readonly DateTime Created = new(2024, 1, 1);
    private static readonly DateTime Updated = new(2024, 2, 1);

    private static ArrangementEntity Build() => new()
    {
        Id = 1,
        CreatedAt = Created,
        UpdatedAt = Updated,
        Name = "Deluxe",
        BasePrice = 49.95m,
        Type = ArrangementType.ThreeCourse
    };

    [Fact]
    public void Equals_IdenticalArrangements_AreEqual()
    {
        var a = Build();
        var b = Build();

        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentName_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Name = "Basic";

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentBasePrice_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.BasePrice = 99.99m;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentType_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Type = ArrangementType.Wine;

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
        Build().Equals("not an arrangement").Should().BeFalse();
    }
}
