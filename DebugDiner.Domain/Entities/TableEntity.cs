class TableEntity
{
    TableEntity
    (
        int id,
        int capacity,
        TableType tableType
    )
    {
        Id = id;
        Capacity = capacity;
        TableType = tableType;
    }
    
    public int Id { get; set; }
    public int Capacity { get; set; }
    public TableType TableType { get; set; }
}