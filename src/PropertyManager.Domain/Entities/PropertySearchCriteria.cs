using PropertyManager.Domain.Enums;

namespace PropertyManager.Domain.Entities;

public class PropertySearchCriteria
{
    public string Type { get; }
    private string City { get; }
    private IReadOnlyList<string> Filters { get; }
    public int Top { get; }
    public string SearchPath { get; }
    
    private PropertySearchCriteria(
        string type, 
        string city, 
        IReadOnlyList<string> filters, 
        int top)
    {
        Type = type;
        City = city;
        Filters = filters;
        Top = top;
        SearchPath = BuildSearchPath(city, filters);
    }
    
    public static PropertySearchCriteria Create(
        PropertyType type, 
        string city, 
        string[]? filters, 
        int top = 10)
    {
        var cleanCity = city.Trim().ToLowerInvariant();
        
        var cleanFilters = filters?
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .Select(f => f.Trim().ToLowerInvariant())
            .ToList() ?? new List<string>();
        
        return new PropertySearchCriteria(
            type.ToString(),
            cleanCity, 
            cleanFilters.AsReadOnly(), 
            top);
    }
    
    private static string BuildSearchPath(string city, IReadOnlyList<string> filters)
    {
        var pathParts = new List<string> { city };
        pathParts.AddRange(filters);
        
        return "/" + string.Join("/", pathParts) + "/";
    }
}