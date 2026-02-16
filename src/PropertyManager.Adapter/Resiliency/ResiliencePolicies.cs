using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using PropertyManager.Adapter.Configuration;

namespace PropertyManager.Adapter.Resiliency;

internal static class ResiliencePolicies
{
    public static IHttpClientBuilder AddResiliencePolicies(
        this IHttpClientBuilder clientBuilder,
        IConfiguration configuration)
    {
        return clientBuilder
            .AddPolicyHandler((serviceProvider, _) => 
                GetRateLimitRetryPolicy(serviceProvider, configuration))
            .AddPolicyHandler((serviceProvider, _) => 
                GetTransientErrorRetryPolicy(serviceProvider, configuration))
            .AddPolicyHandler((serviceProvider, _) => 
                GetTimeoutPolicy(serviceProvider, configuration));
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetRateLimitRetryPolicy(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<PropertyMasterDataClient>>();
        var settings = GetResilienceSettings(configuration);
        
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: settings.RetryCount,
                sleepDurationProvider: _ => TimeSpan.FromMilliseconds(settings.RetryDelayMilliseconds),
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    logger.LogWarning(
                        "Rate limit hit. Retry {RetryAttempt}/{MaxRetries} after {Delay}s",
                        retryAttempt,
                        settings.RetryCount,
                        timespan.TotalSeconds);
                });
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetTransientErrorRetryPolicy(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<PropertyMasterDataClient>>();
        var settings = GetResilienceSettings(configuration);
        
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                retryCount: settings.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    logger.LogWarning(
                        "Transient error. Retry {RetryAttempt}/{MaxRetries} after {Delay}s. Reason: {Reason}",
                        retryAttempt,
                        settings.RetryCount,
                        timespan.TotalSeconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString() ?? "Unknown");
                });
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<PropertyMasterDataClient>>();
        var settings = GetResilienceSettings(configuration);
        
        return Policy.TimeoutAsync<HttpResponseMessage>(
            timeout: TimeSpan.FromMilliseconds(settings.TimeoutMilliseconds),
            onTimeoutAsync: (context, timespan, _, _) =>
            {
                logger.LogWarning("Request timed out after {Timeout}s", timespan.TotalSeconds);
                return Task.CompletedTask;
            });
    }
    
    private static ResilienceSettings GetResilienceSettings(IConfiguration configuration)
    {
        return configuration.GetSection($"{nameof(FundaSettings)}:Resilience").Get<ResilienceSettings>()
            ?? throw new InvalidOperationException("Resilience settings are not properly configured");
    }
}