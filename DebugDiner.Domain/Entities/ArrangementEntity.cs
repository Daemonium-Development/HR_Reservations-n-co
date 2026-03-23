class ArrangementEntity
{
    public ArrangementEntity
    (
        int id,
        string name,
        decimal basePrice,
        ArrangementType arrangementType
    )
    {
        Id = id;
        Name = name;
        BasePrice = basePrice;
        ArrangementType = arrangementType;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public decimal BasePrice { get; set; }
    public ArrangementType ArrangementType { get; set; }
}