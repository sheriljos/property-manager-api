using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PropertyManager.Adapter.Configuration;
using PropertyManager.Adapter.Resiliency;
using PropertyManager.Domain.Ports.Property;

namespace PropertyManager.Adapter;

public static class Bootstrapper
{
    public static IServiceCollection ConfigurePropertyManagerAdapter(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        
        services.Configure<FundaSettings>(configuration.GetSection(nameof(FundaSettings)));
        
        services.AddHttpClient<IPropertyMasterDataClient, PropertyMasterDataClient>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<FundaSettings>>().Value;
                client.BaseAddress = settings.BaseUrl;
            })
            .AddResiliencePolicies(configuration);
        
        return services;
    }
}