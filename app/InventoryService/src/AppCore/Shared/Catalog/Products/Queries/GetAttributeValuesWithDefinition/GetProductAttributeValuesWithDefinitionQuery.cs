namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValuesWithDefinition;

using OysterFx.AppCore.Shared.Queries;

public class GetProductAttributeValuesWithDefinitionQuery : IQuery<GetProductAttributeValuesWithDefinitionQueryResult>
{
    public GetProductAttributeValuesWithDefinitionQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
