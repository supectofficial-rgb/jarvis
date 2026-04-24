namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetDetailsWithAttributes;

using OysterFx.AppCore.Shared.Queries;

public class GetProductDetailsWithAttributesQuery : IQuery<GetProductDetailsWithAttributesQueryResult>
{
    public GetProductDetailsWithAttributesQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
