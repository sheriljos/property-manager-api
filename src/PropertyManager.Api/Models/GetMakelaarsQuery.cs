using System.ComponentModel.DataAnnotations;
using PropertyManager.Domain.Entities;
using PropertyManager.Domain.Enums;

namespace PropertyManager.Models;

public record GetMakelaarsQuery
{
    [Required]
    public required PropertyType Type { get; init; }
    
    [Required]
    public required string City { get; init; }
    
    public string[] Filters { get; init; } = [];
    
    [Range(1, 100)]
    public int Top { get; init; } = 10;
    
    public PropertySearchCriteria ToDomain()
    {
        return PropertySearchCriteria.Create(Type, City, Filters, Top);
    }
}