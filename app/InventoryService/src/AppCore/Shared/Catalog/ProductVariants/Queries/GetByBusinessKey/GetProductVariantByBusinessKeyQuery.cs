namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetProductVariantByBusinessKeyQuery : IQuery<GetProductVariantByBusinessKeyQueryResult>
{
    public GetProductVariantByBusinessKeyQuery(Guid productVariantBusinessKey)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
    }

    public Guid ProductVariantBusinessKey { get; }
}
