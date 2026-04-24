using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.GraphService.AppCore.Domain.GraphRelations.Entities;

public sealed class GraphRelationProjection : AggregateRoot
{
    public string RelationKey { get; private set; } = string.Empty;

    private GraphRelationProjection()
    {
    }

    private GraphRelationProjection(string relationKey)
    {
        RelationKey = relationKey;
    }

    public static GraphRelationProjection Create(string relationKey)
        => new(relationKey);
}

