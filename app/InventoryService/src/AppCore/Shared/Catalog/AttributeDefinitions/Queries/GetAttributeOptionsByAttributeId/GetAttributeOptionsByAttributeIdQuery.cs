namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetAttributeOptionsByAttributeId;

using OysterFx.AppCore.Shared.Queries;

public class GetAttributeOptionsByAttributeIdQuery : IQuery<GetAttributeOptionsByAttributeIdQueryResult>
{
    public GetAttributeOptionsByAttributeIdQuery(Guid attributeDefinitionId)
    {
        AttributeDefinitionId = attributeDefinitionId;
    }

    public Guid AttributeDefinitionId { get; }
}
