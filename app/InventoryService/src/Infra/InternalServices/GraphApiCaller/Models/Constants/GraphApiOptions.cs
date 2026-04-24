namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Constants;

public sealed class GraphApiOptions
{
    public const string Key = "GraphService";

    public bool Enabled { get; set; } = true;
    public string? BaseUrl { get; set; }
    public string UpsertNodePath { get; set; } = "api/GraphService/Graph/nodes";
    public string UpsertRelationPath { get; set; } = "api/GraphService/Graph/relations";
    public int TimeoutMs { get; set; } = 8000;
}
