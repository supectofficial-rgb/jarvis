namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetAttributeDefinitionByBusinessKeyQuery : IQuery<GetAttributeDefinitionByBusinessKeyQueryResult>
{
    public GetAttributeDefinitionByBusinessKeyQuery(Guid attributeDefinitionBusinessKey)
    {
        AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
    }

    public Guid AttributeDefinitionBusinessKey { get; }
}
