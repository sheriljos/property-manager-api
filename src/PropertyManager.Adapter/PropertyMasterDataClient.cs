using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PropertyManager.Adapter.Configuration;
using PropertyManager.Adapter.Dtos;
using PropertyManager.Domain.Entities;
using PropertyManager.Domain.Ports.Property;

namespace PropertyManager.Adapter;

public class PropertyMasterDataClient : IPropertyMasterDataClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PropertyMasterDataClient> _logger;
    private readonly IOptions<FundaSettings> _fundaSettings;
    
    public PropertyMasterDataClient(
        HttpClient httpClient,
        ILogger<PropertyMasterDataClient> logger,
        IOptions<FundaSettings> fundaSettings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _fundaSettings = fundaSettings;
    }
    
    public async Task<List<PropertyMasterData>> GetPropertyDetails(
        PropertySearchCriteria searchCriteria)
    {
        var totalResponse = new List<PropertyMasterData>();
        int currentPage = 1;
        int totalPages;
        
        try
        {
            do
            {
                var responsePerPage = await FetchDataPerPage(searchCriteria, currentPage);
                totalResponse.AddRange(responsePerPage.Objects.Select(obj => obj.ToDomain()));
                totalPages = responsePerPage.Paging.TotalPages;
                currentPage++;
            } while (currentPage <= totalPages);
            
            return totalResponse;
        }
        catch (InvalidOperationException invalidOperationException)
        {
            LogException(searchCriteria, invalidOperationException);
            throw;
        }
        catch (Exception esException)
        {
            LogException(searchCriteria, esException);
            throw;
        }
    }

    private async Task<PropertyMasterDataResponseDto> FetchDataPerPage(
        PropertySearchCriteria searchCriteria, 
        int page)
    {
        var response = await _httpClient.GetAsync(PropertyMasterDataUrl(searchCriteria, page));
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<PropertyMasterDataResponseDto>(content) 
               ?? throw new InvalidOperationException($"Failed to deserialize API response for page {page}");
    }
    
    private void LogException(PropertySearchCriteria searchCriteria, Exception exception)
    {
        _logger.LogError(exception, "Failed to fetch property details for {Type} {SearchPath}", searchCriteria.Type, searchCriteria.SearchPath);
    }
    
    private string PropertyMasterDataUrl(PropertySearchCriteria searchCriteria, int page)
    {
        return $"/feeds/Aanbod.svc/json/{_fundaSettings.Value.ApiKey}/" +
               $"?type={searchCriteria.Type.ToLowerInvariant()}" +
               $"&zo={searchCriteria.SearchPath}" +
               $"&page={page}" +
               $"&pagesize=250";
    }
}