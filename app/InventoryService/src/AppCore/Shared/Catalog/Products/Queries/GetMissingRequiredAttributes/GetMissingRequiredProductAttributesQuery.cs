namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetMissingRequiredAttributes;

using OysterFx.AppCore.Shared.Queries;

public class GetMissingRequiredProductAttributesQuery : IQuery<GetMissingRequiredProductAttributesQueryResult>
{
    public GetMissingRequiredProductAttributesQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
