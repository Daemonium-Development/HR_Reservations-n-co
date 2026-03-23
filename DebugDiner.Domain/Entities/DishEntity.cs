class DishEntity
{
    public DishEntity
    (
        int id,
        string name,
        string description,
        decimal price,
        DishCategory dishCategory
    )
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        DishCategory = dishCategory;    
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DishCategory DishCategory { get; set; }
}