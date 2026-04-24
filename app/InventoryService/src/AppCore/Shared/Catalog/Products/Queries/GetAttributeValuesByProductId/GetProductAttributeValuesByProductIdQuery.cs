namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValuesByProductId;

using OysterFx.AppCore.Shared.Queries;

public class GetProductAttributeValuesByProductIdQuery : IQuery<GetProductAttributeValuesByProductIdQueryResult>
{
    public GetProductAttributeValuesByProductIdQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
