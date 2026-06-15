using FluentAssertions;

namespace DD.Domain.Test;

public class UserEntityTests
{
    private static readonly DateTime Created = new(2024, 1, 1);
    private static readonly DateTime Updated = new(2024, 2, 1);

    private static UserEntity Build() => new()
    {
        Id = 1,
        CreatedAt = Created,
        UpdatedAt = Updated,
        Name = "Alice",
        Email = "alice@example.com",
        PasswordHash = "hash",
        Role = Role.Customer
    };

    [Fact]
    public void Equals_IdenticalUsers_AreEqual()
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
        b.Name = "Bob";

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentEmail_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Email = "bob@example.com";

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentPasswordHash_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.PasswordHash = "other";

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentRole_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Role = Role.Admin;

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
    public void Equals_BaseEntityOfSameKey_IsFalse()
    {
        var user = Build();
        var baseEntity = new BaseEntity { Id = 1, CreatedAt = Created, UpdatedAt = Updated };

        user.Equals(baseEntity).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentType_IsFalse()
    {
        Build().Equals("not a user").Should().BeFalse();
    }
}
