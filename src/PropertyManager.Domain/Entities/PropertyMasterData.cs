namespace PropertyManager.Domain.Entities;

public class PropertyMasterData
{
    public string Id { get; set; } = string.Empty;
    public long MakelaarId { get; set; }
    public string MakelaarName { get; set; } = string.Empty;
}