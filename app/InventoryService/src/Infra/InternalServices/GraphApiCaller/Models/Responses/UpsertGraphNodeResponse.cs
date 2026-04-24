namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Responses;

public sealed class UpsertGraphNodeResponse
{
    public string NodeType { get; set; } = string.Empty;
    public string NodeKey { get; set; } = string.Empty;
}
