using PropertyManager.Domain.Ports.UseCases;
using PropertyManager.Domain.UseCases;

namespace PropertyManager.StartUp;

public  static class DomainExtension
{
    public static IServiceCollection ConfigureDomain(this IServiceCollection services)
    {
        services.AddTransient<IMakelaarsUsecase, MakelaarsUsecase>();
        
        return services;
    }
}