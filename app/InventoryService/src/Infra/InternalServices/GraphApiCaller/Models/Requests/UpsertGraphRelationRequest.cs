namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Requests;

public sealed class UpsertGraphRelationRequest
{
    public string FromNodeType { get; set; } = string.Empty;
    public string FromNodeKey { get; set; } = string.Empty;
    public string ToNodeType { get; set; } = string.Empty;
    public string ToNodeKey { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
    public Dictionary<string, object?> Properties { get; set; } = new(StringComparer.Ordinal);
}
