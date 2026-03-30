using System.ComponentModel.DataAnnotations.Schema;

[Table("user")]
public class UserEntity : BaseEntity
{
    [Column("name")]
    public required string Name { get; set; }
    [Column("email")]
    public required string Email { get; set; }
    [Column("password_hash")]
    public required string PasswordHash { get; set; }
    [Column("role")]
    public required Role Role { get; set; }
}