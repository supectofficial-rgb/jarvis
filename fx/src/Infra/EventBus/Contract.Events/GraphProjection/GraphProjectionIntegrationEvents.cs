namespace OysterFx.Infra.EventBus.Contract.Events.GraphProjection;

using OysterFx.Infra.EventBus.Contract.Events;

[IntegrationEvent("graph.projection.node.upsert.requested.v1")]
public sealed class GraphNodeUpsertRequestedIntegrationEvent
{
    public string NodeType { get; set; } = string.Empty;
    public string NodeKey { get; set; } = string.Empty;
    public Dictionary<string, object?> Properties { get; set; } = new(StringComparer.Ordinal);
}

[IntegrationEvent("graph.projection.relation.upsert.requested.v1")]
public sealed class GraphRelationUpsertRequestedIntegrationEvent
{
    public string FromNodeType { get; set; } = string.Empty;
    public string FromNodeKey { get; set; } = string.Empty;
    public string ToNodeType { get; set; } = string.Empty;
    public string ToNodeKey { get; set; } = string.Empty;
    public string RelationType { get; set; } = "RELATED_TO";
    public Dictionary<string, object?> Properties { get; set; } = new(StringComparer.Ordinal);
}
