using System.ComponentModel.DataAnnotations.Schema;

[Table("reservation")]
public class ReservationEntity : BaseEntity
{
    public override bool Equals(object? obj)
    {
        if (obj is not ReservationEntity entity) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;

        if (!base.Equals(obj)) return false;
        if (UserId != entity.UserId) return false;
        if (TableId != entity.TableId) return false;
        if (StartTime != entity.StartTime) return false;

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(UserId);
        hashCode.Add(TableId);
        hashCode.Add(StartTime);
        return hashCode.ToHashCode();
    }

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
