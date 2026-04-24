namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByBusinessKey;

public class GetAttributeDefinitionByIdQueryResult
{
    public GetAttributeDefinitionByBusinessKeyQueryResult? Item { get; set; }
}
