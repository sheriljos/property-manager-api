using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Adapter.Configuration;

public class FundaSettings
{
    [Required]
    public required Uri BaseUrl { get; init; }
    
    [Required] 
    public required string ApiKey { get; init; }

    [Required]
    public required ResilienceSettings Resilience { get; init; }
}