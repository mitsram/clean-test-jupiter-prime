namespace JupiterPrime.Domain.Entities;

public class Pet
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public List<string>? PhotoUrls { get; set; }
    public string? Status { get; set; }
    public List<Category>? Categories { get; set; }
    public Category? Category { get; set; }
    public List<Tag>? Tags { get; set; }
}

public class Category
{
    public long Id { get; set; }
    public string? Name { get; set; }
}

public class Tag
{
    public long Id { get; set; }
    public string? Name { get; set; }
}

