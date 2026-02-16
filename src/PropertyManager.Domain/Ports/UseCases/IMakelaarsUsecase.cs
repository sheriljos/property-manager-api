using PropertyManager.Domain.Entities;

namespace PropertyManager.Domain.Ports.UseCases;

public interface IMakelaarsUsecase
{
    Task<List<Makelaar>> GetMakelaars(PropertySearchCriteria searchCriteria);
}