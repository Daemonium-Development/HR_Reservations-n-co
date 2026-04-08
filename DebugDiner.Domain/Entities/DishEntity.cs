using System.ComponentModel.DataAnnotations.Schema;

[Table("dishes")]
public class DishEntity : BaseEntity
{
    public override bool Equals(object? obj)
    {
        if (obj is not DishEntity entity) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;

        if (!base.Equals(obj)) return false;

        if (Name != entity.Name) return false;
        if (Description != entity.Description) return false;
        if (Price != entity.Price) return false;
        if (DishCategory != entity.DishCategory) return false;
        if (AllergenInfo != entity.AllergenInfo) return false;

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(Name);
        hashCode.Add(Description);
        hashCode.Add(Price);
        hashCode.Add(DishCategory);
        hashCode.Add(AllergenInfo);
        return hashCode.ToHashCode();
    }

    [Column("name")]
    public required string Name { get; set; }
    [Column("description")]
    public required string Description { get; set; }
    [Column("price")]
    public required decimal Price { get; set; }
    [Column("category")]
    public required DishCategory DishCategory { get; set; }
    [Column("allergen_info")]
    public string AllergenInfo { get; set; } = "";
}
