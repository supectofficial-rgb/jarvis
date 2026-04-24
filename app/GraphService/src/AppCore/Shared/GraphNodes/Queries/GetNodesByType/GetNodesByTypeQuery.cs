namespace Insurance.GraphService.AppCore.Shared.GraphNodes.Queries.GetNodesByType;

using OysterFx.AppCore.Shared.Queries;

public sealed class GetNodesByTypeQuery : IQuery<GetNodesByTypeQueryResult>
{
    public string NodeType { get; set; } = string.Empty;
    public int Limit { get; set; } = 100;
    public Dictionary<string, string>? EqualsFilters { get; set; }
}

public sealed class GetNodesByTypeQueryResult
{
    public string NodeType { get; set; } = string.Empty;
    public List<GraphNodeQueryItem> Items { get; set; } = new();
}

public sealed class GraphNodeQueryItem
{
    public string NodeKey { get; set; } = string.Empty;
    public Dictionary<string, object?> Properties { get; set; } = new(StringComparer.Ordinal);
}

