namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetActiveAttributeOptionsByAttributeId;

using OysterFx.AppCore.Shared.Queries;

public class GetActiveAttributeOptionsByAttributeIdQuery : IQuery<GetActiveAttributeOptionsByAttributeIdQueryResult>
{
    public GetActiveAttributeOptionsByAttributeIdQuery(Guid attributeDefinitionId)
    {
        AttributeDefinitionId = attributeDefinitionId;
    }

    public Guid AttributeDefinitionId { get; }
}
