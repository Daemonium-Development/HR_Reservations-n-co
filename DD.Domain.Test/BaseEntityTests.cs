using FluentAssertions;

namespace DD.Domain.Test;

public class BaseEntityTests
{
    private static readonly DateTime Created = new(2024, 1, 1);
    private static readonly DateTime Updated = new(2024, 2, 1);

    private static BaseEntity Build() => new()
    {
        Id = 1,
        CreatedAt = Created,
        UpdatedAt = Updated
    };

    [Fact]
    public void Equals_IdenticalEntities_AreEqual()
    {
        var a = Build();
        var b = Build();

        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentId_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Id = 2;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentCreatedAt_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.CreatedAt = Created.AddDays(1);

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentUpdatedAt_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.UpdatedAt = Updated.AddDays(1);

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_Null_IsFalse()
    {
        Build().Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentType_IsFalse()
    {
        Build().Equals("not an entity").Should().BeFalse();
    }

    [Fact]
    public void Equals_SameReference_IsTrue()
    {
        var a = Build();

        a.Equals(a).Should().BeTrue();
    }
}
