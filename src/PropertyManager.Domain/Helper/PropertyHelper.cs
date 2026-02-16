using PropertyManager.Domain.Entities;

namespace PropertyManager.Domain.Helper;

public static class PropertyHelper
{
    public static List<Makelaar> GetRankedMakelaars(List<PropertyMasterData> propertyMasterData, int top)
    {
        return propertyMasterData
            .GroupBy(p => p.MakelaarId)
            .Select(g => new Makelaar
            {
                Id = g.Key,
                Name = g.First().MakelaarName,
                NumberOfProperties = g.Count()
            })
            .OrderByDescending(r => r.NumberOfProperties)
            .Take(top)
            .ToList();
    }
}