using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("table")]
class TableEntity : BaseEntity
{
    [Column("capacity")]
    public required int Capacity { get; set; }
    [Column("type")]
    public required TableType Type { get; set; }
}