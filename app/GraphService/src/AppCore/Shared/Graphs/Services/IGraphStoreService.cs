namespace Insurance.GraphService.AppCore.Shared.Graphs.Services;

public interface IGraphStoreService
{
    Task UpsertNodeAsync(GraphNodeUpsertRequest request, CancellationToken cancellationToken = default);
    Task UpsertRelationAsync(GraphRelationUpsertRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GraphNodeView>> GetNodesByTypeAsync(
        string nodeType,
        int limit,
        IReadOnlyDictionary<string, string>? equalsFilters,
        CancellationToken cancellationToken = default);
}

public sealed record GraphNodeUpsertRequest(
    string NodeType,
    string NodeKey,
    IReadOnlyDictionary<string, object?> Properties);

public sealed record GraphRelationUpsertRequest(
    string FromNodeType,
    string FromNodeKey,
    string ToNodeType,
    string ToNodeKey,
    string RelationType,
    IReadOnlyDictionary<string, object?> Properties);

public sealed record GraphNodeView(
    string NodeType,
    string NodeKey,
    Dictionary<string, object?> Properties);
