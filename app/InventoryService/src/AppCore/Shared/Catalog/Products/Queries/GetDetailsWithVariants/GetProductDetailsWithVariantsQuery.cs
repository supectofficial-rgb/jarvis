namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetDetailsWithVariants;

using OysterFx.AppCore.Shared.Queries;

public class GetProductDetailsWithVariantsQuery : IQuery<GetProductDetailsWithVariantsQueryResult>
{
    public GetProductDetailsWithVariantsQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
