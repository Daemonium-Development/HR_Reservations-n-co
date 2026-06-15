using FluentAssertions;

namespace DD.Domain.Test;

public class DishEntityTests
{
    private static readonly DateTime Created = new(2024, 1, 1);
    private static readonly DateTime Updated = new(2024, 2, 1);

    private static DishEntity Build() => new()
    {
        Id = 1,
        CreatedAt = Created,
        UpdatedAt = Updated,
        Name = "Steak",
        Description = "Grilled steak",
        Price = 24.50m,
        DishCategory = DishCategory.Meat,
        AllergenInfo = "Gluten"
    };

    [Fact]
    public void Equals_IdenticalDishes_AreEqual()
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
        b.Name = "Salad";

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentDescription_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Description = "Something else";

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentPrice_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.Price = 9.99m;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentCategory_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.DishCategory = DishCategory.Vegan;

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentAllergenInfo_AreNotEqual()
    {
        var a = Build();
        var b = Build();
        b.AllergenInfo = "Nuts";

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DefaultAllergenInfoOnBoth_AreEqual()
    {
        var a = new DishEntity
        {
            Id = 1,
            CreatedAt = Created,
            UpdatedAt = Updated,
            Name = "Steak",
            Description = "Grilled steak",
            Price = 24.50m,
            DishCategory = DishCategory.Meat
        };
        var b = new DishEntity
        {
            Id = 1,
            CreatedAt = Created,
            UpdatedAt = Updated,
            Name = "Steak",
            Description = "Grilled steak",
            Price = 24.50m,
            DishCategory = DishCategory.Meat
        };

        a.AllergenInfo.Should().Be("");
        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentClrType_IsFalse()
    {
        Build().Equals("not a dish").Should().BeFalse();
    }
}
