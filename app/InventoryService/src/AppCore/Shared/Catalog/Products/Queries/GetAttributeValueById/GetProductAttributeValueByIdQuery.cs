namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValueById;

using OysterFx.AppCore.Shared.Queries;

public class GetProductAttributeValueByIdQuery : IQuery<GetProductAttributeValueByIdQueryResult>
{
    public GetProductAttributeValueByIdQuery(Guid productAttributeValueId)
    {
        ProductAttributeValueId = productAttributeValueId;
    }

    public Guid ProductAttributeValueId { get; }
}
