using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("dishes")]
class DishEntity : BaseEntity
{
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