using System.ComponentModel.DataAnnotations.Schema;

[Table("table")]
public class TableEntity : BaseEntity
{
    public override bool Equals(object? obj)
    {
        if (obj is not TableEntity entity) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;
        
        if (!base.Equals(obj)) return false;
        if (Capacity != entity.Capacity) return false;
        if (Type != entity.Type) return false;
        
        return true;
    }
    
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(Capacity);
        hashCode.Add(Type);
        return hashCode.ToHashCode();
    }

    [Column("capacity")]
    public required int Capacity { get; set; }
    [Column("type")]
    public required TableType Type { get; set; }
}