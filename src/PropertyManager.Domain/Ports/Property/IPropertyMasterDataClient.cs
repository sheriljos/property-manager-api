using PropertyManager.Domain.Entities;

namespace PropertyManager.Domain.Ports.Property;

public interface IPropertyMasterDataClient
{
    Task<List<PropertyMasterData>> GetPropertyDetails(PropertySearchCriteria serachCriteria);
}