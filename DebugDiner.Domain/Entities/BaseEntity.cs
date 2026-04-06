using System.ComponentModel.DataAnnotations.Schema;

public class BaseEntity
{
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity entity) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;

        if (Id != entity.Id) return false;
        if (CreatedAt != entity.CreatedAt) return false;
        if (UpdatedAt != entity.UpdatedAt) return false;

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(CreatedAt);
        hashCode.Add(UpdatedAt);
        return hashCode.ToHashCode();
    }

    public required int Id { get; set; }
    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
