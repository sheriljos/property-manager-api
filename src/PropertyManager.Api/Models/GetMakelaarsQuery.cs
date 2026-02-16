using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PropertyManager.Domain.Entities;
using PropertyManager.Domain.Enums;

namespace PropertyManager.Models;

public record GetMakelaarsQuery
{
    [FromQuery(Name = "type")] public required PropertyType Type { get; init; } = PropertyType.koop;
    
    [FromQuery(Name = "city")]
    [Required]
    public required string City { get; init; }
    
    [FromQuery(Name = "filters")]
    public string[] Filters { get; init; } = [];
    
    [FromQuery(Name = "top")]
    [Range(1, 100)]
    public int Top { get; init; } = 10;
    
    public PropertySearchCriteria ToDomain()
    {
        return PropertySearchCriteria.Create(Type, City, Filters, Top);
    }
}