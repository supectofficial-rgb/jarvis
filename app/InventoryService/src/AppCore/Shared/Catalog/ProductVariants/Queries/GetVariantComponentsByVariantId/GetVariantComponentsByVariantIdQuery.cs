namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantComponentsByVariantId;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantComponentsByVariantIdQuery : IQuery<GetVariantComponentsByVariantIdQueryResult>
{
    public GetVariantComponentsByVariantIdQuery(Guid variantId)
    {
        VariantId = variantId;
    }

    public Guid VariantId { get; }
}
