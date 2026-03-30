using System.ComponentModel.DataAnnotations.Schema;

[Table("arrangements")]
public class ArrangementEntity : BaseEntity
{
    [Column("name")]
    public required string Name { get; set; }
    [Column("base_price")]
    public required decimal BasePrice { get; set; }
    [Column("type")]
    public required ArrangementType Type { get; set; }
}