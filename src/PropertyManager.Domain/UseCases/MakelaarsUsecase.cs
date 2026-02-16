using PropertyManager.Domain.Entities;
using PropertyManager.Domain.Helper;
using PropertyManager.Domain.Ports.Property;
using PropertyManager.Domain.Ports.UseCases;

namespace PropertyManager.Domain.UseCases;
public class MakelaarsUsecase : IMakelaarsUsecase
{
    private readonly IPropertyMasterDataClient _propertyMasterDataClient;
    
    public MakelaarsUsecase(IPropertyMasterDataClient propertyMasterDataClient)
    {
        _propertyMasterDataClient = propertyMasterDataClient;
    }
    
    public async Task<List<Makelaar>> GetMakelaars(PropertySearchCriteria serachCriteria)
    {
        var propertyMasterData = await _propertyMasterDataClient.GetPropertyDetails(serachCriteria);

        return PropertyHelper.GetRankedMakelaars(propertyMasterData, serachCriteria.Top);
    }
}