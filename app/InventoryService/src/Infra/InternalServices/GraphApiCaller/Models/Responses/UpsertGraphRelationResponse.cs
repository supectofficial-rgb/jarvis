namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Responses;

public sealed class UpsertGraphRelationResponse
{
    public string RelationType { get; set; } = string.Empty;
    public string FromNodeKey { get; set; } = string.Empty;
    public string ToNodeKey { get; set; } = string.Empty;
}
