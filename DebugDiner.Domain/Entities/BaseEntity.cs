using System.ComponentModel.DataAnnotations.Schema;

public class BaseEntity
{
    public required int Id {get;set;}
    [Column("created_at")]
    public required DateTime CreatedAt {get;set;}
    [Column("updated_at")]
    public DateTime UpdatedAt {get;set;}
}