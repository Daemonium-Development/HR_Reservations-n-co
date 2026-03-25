using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("reservations")]
class ReservationEntity : BaseEntity
{
    [Column("user_id")]
    public required int UserId { get; set; }
    [Column("table_id")]
    public required int TableId { get; set; } 
    [Column("start_time")]
    public required DateTime StartTime { get; set; }
    [Column("end_time")]
    public required DateTime EndTime { get; set; }
    [Column("guests")]
    public required int Guests { get; set; }
    [Column("status")]
    public required ReservationStatus Status { get; set; }
}