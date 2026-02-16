namespace PropertyManager.Domain.Entities;

public class Makelaar
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NumberOfProperties { get; set; }
}