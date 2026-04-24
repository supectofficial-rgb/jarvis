namespace Insurance.InventoryService.AppCore.Shared.Catalog.Services;

public sealed record GraphProjectionNodeRequest(
    string NodeType,
    string NodeKey,
    IReadOnlyDictionary<string, object?> Properties);

public sealed record GraphProjectionRelationRequest(
    string FromNodeType,
    string FromNodeKey,
    string ToNodeType,
    string ToNodeKey,
    string RelationType,
    IReadOnlyDictionary<string, object?> Properties);

public interface IGraphProjectionService
{
    Task UpsertNodeAsync(GraphProjectionNodeRequest request, CancellationToken cancellationToken = default);
    Task UpsertRelationAsync(GraphProjectionRelationRequest request, CancellationToken cancellationToken = default);
}
