namespace Apbd4;

public class Animal
{
    public Animal(int ıd, string name, string category, double weight, string furColor)
    {
        Id = ıd;
        Name = name;
        Category = category;
        Weight = weight;
        FurColor = furColor;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public double Weight { get; set; }
    public string FurColor { get; set; } 
    
}

public class Visit
{
    public int Id { get; set; }
    public DateTime DateOfVisit { get; set; }
    public int AnimalId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    
}
