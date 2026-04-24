namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Requests;

public sealed class UpsertGraphNodeRequest
{
    public string NodeType { get; set; } = string.Empty;
    public string NodeKey { get; set; } = string.Empty;
    public Dictionary<string, object?> Properties { get; set; } = new(StringComparer.Ordinal);
}
