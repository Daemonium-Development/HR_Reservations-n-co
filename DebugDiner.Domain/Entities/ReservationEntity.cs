class ReservationEntity
{
    public ReservationEntity
    (
        int id,
        DateTime startTime,
        DateTime endTime,
        int numberOfGuests,
        ReservationStatus reservationStatus,
        int userId,
        int tableId,
        List<int> arrangementIds
    )
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        NumberOfGuests = numberOfGuests;
        ReservationStatus = reservationStatus;
        UserId = userId;
        TableId = tableId;
        ArrangementIds = arrangementIds;
    }

    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int NumberOfGuests { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
    public int UserId { get; set; }
    public int TableId { get; set; }
    public List<int> ArrangementIds { get; set; }

}