using PropertyManager.Domain.Entities;

namespace PropertyManager.Models;

public class MakelaarResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NumberOfProperties {get; set;}
    
    public static MakelaarResponseDto FromDomain(Makelaar makelaar) =>
        new ()
        {
            Id = makelaar.Id,
            Name = makelaar.Name,
            NumberOfProperties = makelaar.NumberOfProperties,
        };
}