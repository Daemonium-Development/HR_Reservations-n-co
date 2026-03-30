using System.ComponentModel.DataAnnotations.Schema;

[Table("user")]
public class UserEntity : BaseEntity
{
    public override bool Equals(object? obj)
    {
        if (obj is not UserEntity entity) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;

        if (!base.Equals(obj)) return false;
        if (Name != entity.Name) return false;
        if (Email != entity.Email) return false;
        if (PasswordHash != entity.PasswordHash) return false;
        if (Role != entity.Role) return false;

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(Name);
        hashCode.Add(Email);
        hashCode.Add(PasswordHash);
        hashCode.Add(Role);
        return hashCode.ToHashCode();
    }

    [Column("name")]
    public required string Name { get; set; }
    [Column("email")]
    public required string Email { get; set; }
    [Column("password_hash")]
    public required string PasswordHash { get; set; }
    [Column("role")]
    public required Role Role { get; set; }
}