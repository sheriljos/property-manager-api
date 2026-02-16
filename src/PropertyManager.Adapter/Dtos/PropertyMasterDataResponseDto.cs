using System.Text.Json.Serialization;
using PropertyManager.Domain.Entities;

namespace PropertyManager.Adapter.Dtos;

public class PropertyMasterDataResponseDto
{
    [JsonPropertyName("Objects")]
    public List<PropertyMasterDataDto> Objects { get; set; } = new();
    
    [JsonPropertyName("Paging")]
    public PagingInfo Paging { get; set; } = new();
    
    [JsonPropertyName("TotaalAantalObjecten")]
    public int TotalCount { get; set; }
}

public class PagingInfo
{
    [JsonPropertyName("AantalPaginas")]
    public int TotalPages { get; set; }
    
    [JsonPropertyName("HuidigePagina")]
    public int CurrentPage { get; set; }
}

public class PropertyMasterDataDto
{
    [JsonPropertyName("Id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("MakelaarId")]
    public long MakelaarId { get; init; }
    
    [JsonPropertyName("MakelaarNaam")]
    public string MakelaarName { get; init; } = string.Empty;

    public PropertyMasterData ToDomain() =>
        new()
        {
            Id = Id,
            MakelaarId = MakelaarId,
            MakelaarName = MakelaarName
        };
}