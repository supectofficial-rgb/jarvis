namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetAttributeDefinitionByIdQuery : IQuery<GetAttributeDefinitionByIdQueryResult>
{
    public GetAttributeDefinitionByIdQuery(Guid attributeDefinitionId)
    {
        AttributeDefinitionId = attributeDefinitionId;
    }

    public Guid AttributeDefinitionId { get; }
}
