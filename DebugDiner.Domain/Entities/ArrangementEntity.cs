using System.ComponentModel.DataAnnotations.Schema;

[Table("arrangements")]
public class ArrangementEntity : BaseEntity
{
    public override bool Equals(object? obj)
    {
        if (obj is not ArrangementEntity entity) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;
        
        if (!base.Equals(obj)) return false;
        
        if (Name != entity.Name) return false;
        if (BasePrice != entity.BasePrice) return false;
        if (Type != entity.Type) return false;
        
        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(Name);
        hashCode.Add(BasePrice);
        hashCode.Add(Type);
        return hashCode.ToHashCode();
    }

    [Column("name")]
    public required string Name { get; set; }
    [Column("base_price")]
    public required decimal BasePrice { get; set; }
    [Column("type")]
    public required ArrangementType Type { get; set; }
}