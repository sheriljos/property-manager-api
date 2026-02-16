using System.ComponentModel.DataAnnotations;

namespace PropertyManager.Adapter.Configuration;

public class ResilienceSettings
{
    [Required]
    public required int RetryCount { get; init; }
    
    [Required]
    public required int RetryDelayMilliseconds { get; init; }
    
    [Required]
    public required int TimeoutMilliseconds { get; init; }
}